using Core.GlobalGameState.Services;
using Core.SaveSystemBase;
using Core.SaveSystemBase.Data;

using Cysharp.Threading.Tasks;

using SO.PlayerConfigs;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

#if UNITY_WEBGL
using YG;
#endif

namespace Core.GlobalGameState
{
    public class PlayerState
    {
        private readonly string _playerSaveDataKey = "Player_Data";
        private CancellationTokenSource _lifetimeCts = new();

        private PlayerEconomyService _playerEconomyService;
        private PlayerUpgradeService _playerUpgradeService;
        private PlayerBonusesService _playerBonusesService;
        private PlayerRewardsByLevelService _playerRewardsByLevelService;
        private ShopState _shopState;

        private bool _isInitialized = false;

        private readonly SaveSystemContext _saveSystemContext;

        private readonly MainEconomyConfig _economyConfig;
        private readonly RewardsByLevelConfig _rewardsByLevelConfig;
        private readonly CyclicRewardsConfig _cyclicRewardsConfig;
        private readonly PlayerConfig _playerConfig;

        public PlayerEconomyService EconomyService => _playerEconomyService;
        public PlayerUpgradeService UpgradeService => _playerUpgradeService;
        public PlayerBonusesService BonusesService => _playerBonusesService;
        public PlayerRewardsByLevelService RewardsService => _playerRewardsByLevelService;
        public ShopState ShopState => _shopState;

        public PlayerState(SaveSystemContext saveSystemContext)
        {
#if UNITY_WEBGL
            YG2.onGetSDKData += HandleSDKData;
            Debug.Log("[Player State] Player state initialized");
#endif
            _saveSystemContext = saveSystemContext;

            _economyConfig = Resources.Load<MainEconomyConfig>("Configs/Economy/MainEconomyConfig");
            _playerConfig = Resources.Load<PlayerConfig>("Configs/Player/PlayerConfig");
            _rewardsByLevelConfig = Resources.Load<RewardsByLevelConfig>("Configs/Player/RewardsByLevelConfig");
            _cyclicRewardsConfig = Resources.Load<CyclicRewardsConfig>("Configs/Player/CyclicRewardsConfig");
        }

        private async UniTask InitializeAsync()
        {
            try
            {
#if UNITY_WEBGL
                await UniTask.WaitUntil(() => YG2.isSDKEnabled).AttachExternalCancellation(_lifetimeCts.Token);
                Debug.Log("[Player State] YG2 SDK Enabled. Checking saves...");
#endif

                bool hasSavedData = false;

#if UNITY_WEBGL
                hasSavedData = YG2.saves.PlayerData != null;
#else
            hasSavedData = PlayerPrefs.HasKey(_playerSaveDataKey);
#endif

#if UNITY_EDITOR
                if (_playerConfig.IsDebug)
                {
                    _saveSystemContext.Delete(_playerSaveDataKey);
                    hasSavedData = false;
                }
#endif

                if (!hasSavedData)
                {
                    _playerBonusesService = new(_playerConfig);
                    _playerEconomyService = new(_economyConfig, _playerBonusesService.BonusStateChanged, _playerConfig.BonusClickMultiplier);
                    _playerUpgradeService = new(_playerEconomyService);
                    _playerRewardsByLevelService = new(_rewardsByLevelConfig, _cyclicRewardsConfig, _playerBonusesService.LevelChanged, _playerEconomyService);

                    _shopState = new(_playerUpgradeService);

                    Debug.Log("[Player State] New player data created");
                }
                else
                    LoadPlayerState(_economyConfig, _playerConfig, _rewardsByLevelConfig, _cyclicRewardsConfig);

                _isInitialized = true;
                Debug.Log("[Player State] Player State fully initialized");
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("[Player State] Initialization cancelled");
                throw;
            }
        }

        public async UniTask StartAsyncTasks()
        {
            await InitializeAsync();

            _playerEconomyService.StartAsyncTasks();
            _playerBonusesService.StartAsyncDecreaseTask();
        }

        private void LoadPlayerState(
            MainEconomyConfig economyConfig,
            PlayerConfig playerConfig,
            RewardsByLevelConfig rewardsByLevelConfig,
            CyclicRewardsConfig cyclicRewardsConfig)
        {
            PlayerData loadedData = _saveSystemContext.Load(_playerSaveDataKey);

            _playerBonusesService = new(playerConfig, loadedData);
            _playerEconomyService = new(economyConfig, _playerBonusesService.BonusStateChanged, playerConfig.BonusClickMultiplier, loadedData);
            _playerUpgradeService = new(_playerEconomyService);
            _playerRewardsByLevelService = new(rewardsByLevelConfig, cyclicRewardsConfig, _playerBonusesService.LevelChanged, _playerEconomyService);
            _shopState = new(_playerUpgradeService);

            _shopState.Restore(loadedData.ShopsData);
        }

        private void SavePlayerState()
        {
            PlayerData playerData = new()
            {
                Coins = _playerEconomyService.CoinsWallet.Amount,
                Gems = _playerEconomyService.GemsWallet.Amount,

                PlayerClickAmount = _playerEconomyService.PlayerClickAmount,
                PassiveIncomeAmount = _playerEconomyService.PassiveIncomeAmount,

                TrippleClickChance = _playerEconomyService.TrippleClickChance,

                Level = _playerBonusesService.Level,
                CurrentExp = _playerBonusesService.CurrentExp,

                GainedExpPerClick = _playerBonusesService.GainedExpPerClick,
                ExpToLevelUp = _playerBonusesService.ExpToLevelUp,

                ShopsData = CreateShopsData(),
                ReceivedRewards = CreateRewardsData(),
                CyclicRewards = CreateCyclicRewardsData()
            };

            _saveSystemContext.Save(playerData, _playerSaveDataKey);
        }

        private List<RewardData> CreateRewardsData()
        {
            List<RewardData> receivedRewardsData = new();

            foreach (var reward in _playerRewardsByLevelService.RewardsDict.Values)
            {
                RewardData rewardData = new()
                {
                    ID = reward.RewardID,
                    Received = reward.IsReceived
                };
                receivedRewardsData.Add(rewardData);
            }

            return receivedRewardsData;
        }

        private List<RewardData> CreateCyclicRewardsData()
        {
            List<RewardData> cyclicRewardsData = new();

            foreach (var reward in _playerRewardsByLevelService.CyclicRewardsDict.Values)
            {
                RewardData rewardData = new()
                {
                    ID = reward.RewardID,
                    Received = reward.IsReceived
                };
                cyclicRewardsData.Add(rewardData);
            }

            return cyclicRewardsData;
        }

        private List<ShopStateData> CreateShopsData() => _shopState.Capture();

#if UNITY_WEBGL
        private void HandleSDKData()
        {
            Debug.Log("SDK Data Enabled");
        }
#endif

        public void StopAsyncTasks()
        {
            _playerEconomyService.StopAsyncTasks();
            _playerBonusesService.StopAsyncDecreaseTask();
        }

        public void Dispose()
        {
            SavePlayerState();
#if UNITY_WEBGL
            YG2.onGetSDKData -= HandleSDKData;
#endif
            _lifetimeCts.Cancel();
            _lifetimeCts.Dispose();

            _playerEconomyService?.Dispose();
            _playerRewardsByLevelService?.Dispose();
            _shopState?.Dispose();
        }
    }
}