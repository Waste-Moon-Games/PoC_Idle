using Core.GlobalGameState.Services;
using Core.SaveSystemBase;
using Core.SaveSystemBase.Data;

using Cysharp.Threading.Tasks;
using R3;
using SO.AdsConfigs;
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
        private readonly CompositeDisposable _disposables = new();

        private CancellationTokenSource _lifetimeCts = new();
#if UNITY_WEBGL
        private readonly UniTaskCompletionSource _sdkDataReadyTcs = new();
#endif
        private readonly AutoSaveService _autoSaveService;
        private readonly PlayerRewardBonusesService _rewardBonuses;

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
        public PlayerRewardBonusesService PlayerRewardBonusesService => _rewardBonuses;

        public PlayerState(SaveSystemContext saveSystemContext)
        {
#if UNITY_WEBGL
            YG2.onGetSDKData += HandleSDKData;
#endif
            _saveSystemContext = saveSystemContext;

            _economyConfig = Resources.Load<MainEconomyConfig>("Configs/Economy/MainEconomyConfig");
            _playerConfig = Resources.Load<PlayerConfig>("Configs/Player/PlayerConfig");
            _rewardsByLevelConfig = Resources.Load<RewardsByLevelConfig>("Configs/Player/RewardsByLevelConfig");
            _cyclicRewardsConfig = Resources.Load<CyclicRewardsConfig>("Configs/Player/CyclicRewardsConfig");
            var rewardAdsConfig = Resources.Load<RewardAdsConfig>("Configs/Ads/RewardAdsConfig");

            var autoSaveToken = new CancellationTokenSource();
            _autoSaveService = new(_playerConfig.AutoSaveDelay, autoSaveToken);
            _autoSaveService.AutoSaveSignal.Subscribe(def => SavePlayerState()).AddTo(_disposables);

            _rewardBonuses = new(rewardAdsConfig.InitTemporaryBonusDurationInMinutes);
        }

        public async UniTask InitializeAsync()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("[Player State] InitiailzeAsync called more than once. Skipping repeated initialization.");
                return;
            }

            try
            {
#if UNITY_WEBGL
                await UniTask.WaitUntil(() => YG2.isSDKEnabled)
                    .AttachExternalCancellation(_lifetimeCts.Token);
#endif
                bool hasSavedData = HasSavedData();
#if UNITY_EDITOR
                if (_playerConfig.IsDebug)
                {
                    _saveSystemContext.Delete(_playerSaveDataKey);
                    hasSavedData = false;
                }
#endif

                if (hasSavedData)
                {
                    LoadPlayerState(_economyConfig, _playerConfig, _rewardsByLevelConfig, _cyclicRewardsConfig);
                }
                else
                {
                    CreateNewPlayerState();
                }

                _isInitialized = true;
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("[Player State] Initialization cancelled");
                throw;
            }
        }

        private void CreateNewPlayerState()
        {
            _playerBonusesService = new(_playerConfig);
            _playerEconomyService = new(_economyConfig, _playerBonusesService.BonusStateChanged, _rewardBonuses.TemporaryBonusStateChanged, _playerConfig.BonusClickMultiplier);
            _playerUpgradeService = new(_playerEconomyService);
            _playerRewardsByLevelService = new(_rewardsByLevelConfig, _cyclicRewardsConfig, _playerBonusesService.LevelChanged, _playerEconomyService);

            _shopState = new(_playerUpgradeService);
        }

        public void StartAsyncOperations()
        {
            _playerEconomyService.StartAsyncTasks();
            _playerBonusesService.StartAsyncDecreaseTask();
            _autoSaveService.AsyncAutoSave().Forget();
        }

        private void LoadPlayerState(
            MainEconomyConfig economyConfig,
            PlayerConfig playerConfig,
            RewardsByLevelConfig rewardsByLevelConfig,
            CyclicRewardsConfig cyclicRewardsConfig)
        {
            PlayerData loadedData = _saveSystemContext.Load(_playerSaveDataKey);

            if (loadedData == null)
            {
                Debug.LogWarning("[Player State] Save data is missing or corrupted. Creating new player state.");
                CreateNewPlayerState();
                return;
            }

            _playerBonusesService = new(playerConfig, loadedData);
            _playerEconomyService = new(economyConfig, _playerBonusesService.BonusStateChanged, _rewardBonuses.TemporaryBonusStateChanged, playerConfig.BonusClickMultiplier, loadedData);
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

        private bool HasSavedData()
        {
#if UNITY_WEBGL
            string jsonData = YG2.saves?.JsonData;
            Debug.Log($"[Player State] Cloud player data present: {!string.IsNullOrEmpty(jsonData)}");
            return !string.IsNullOrEmpty(jsonData);
#else
            return PlayerPrefs.HasKey(_playerSaveDataKey);
#endif
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
            _sdkDataReadyTcs.TrySetResult();
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
            _lifetimeCts = null;

            _disposables.Dispose();

            _playerEconomyService?.Dispose();
            _playerRewardsByLevelService?.Dispose();
            _shopState?.Dispose();
            _autoSaveService?.Dispose();
        }
    }
}