using Core.GlobalGameState.Services;
using Core.SaveSystemBase;
using Core.SaveSystemBase.Data;
using Core.Shop.Base;
using SO.PlayerConfigs;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using YG;
#endif

namespace Core.GlobalGameState
{
    public class PlayerState
    {
        private readonly string _playerSaveDataKey = "Player_Data";

        private PlayerEconomyService _playerEconomyService;
        private PlayerUpgradeService _playerUpgradeService;
        private PlayerBonusesService _playerBonusesService;
        private PlayerRewardsByLevelService _playerRewardsByLevelService;
        private ShopState _shopState;

        private readonly SaveSystemContext _saveSystemContext;

        public PlayerEconomyService EconomyService => _playerEconomyService;
        public PlayerUpgradeService UpgradeService => _playerUpgradeService;
        public PlayerBonusesService BonusesService => _playerBonusesService;
        public PlayerRewardsByLevelService RewardsService => _playerRewardsByLevelService;
        public ShopState ShopState => _shopState;

        public PlayerState(SaveSystemContext saveSystemContext)
        {
            _saveSystemContext = saveSystemContext;

            var economyConfig = Resources.Load<MainEconomyConfig>("Configs/Economy/MainEconomyConfig");
            var playerConfig = Resources.Load<PlayerConfig>("Configs/Player/PlayerConfig");
            var rewardsByLevelConfig = Resources.Load<RewardsByLevelConfig>("Configs/Player/RewardsByLevelConfig");
            var cyclicRewardsConfig = Resources.Load<CyclicRewardsConfig>("Configs/Player/CyclicRewardsConfig");

            bool hasSavedData = false;
#if UNITY_WEBGL && !UNITY_EDITOR
            hasSavedData = !string.IsNullOrEmpty(YG.YG2.saves.JsonData);
#else
            hasSavedData = PlayerPrefs.HasKey(_playerSaveDataKey);
#endif
            if (hasSavedData)
            {
                LoadPlayerState(economyConfig, playerConfig, rewardsByLevelConfig, cyclicRewardsConfig);
                return;
            }
            _playerBonusesService = new(playerConfig);
            _playerEconomyService = new(economyConfig, _playerBonusesService.BonusStateChanged, playerConfig.BonusClickMultiplier);
            _playerUpgradeService = new(_playerEconomyService);
            _playerRewardsByLevelService = new(rewardsByLevelConfig, cyclicRewardsConfig, _playerBonusesService.LevelChanged, _playerEconomyService);
            
            _shopState = new(_playerUpgradeService);
        }

        public void StartAsyncTasks()
        {
            _playerEconomyService.StartAsyncTasks();
            _playerBonusesService.StartAsyncDecreaseTask();
        }

        private void LoadPlayerState(
            MainEconomyConfig economyConfig,
            PlayerConfig playerConfig,
            RewardsByLevelConfig rewardsByLevelConfig,
            CyclicRewardsConfig cyclicRewardsConfig)
        {
            PlayerData loadedData = _saveSystemContext.Load(_playerSaveDataKey, new PlayerData());

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

        public void StopAsyncTasks()
        {
            _playerEconomyService.StopAsyncTasks();
            _playerBonusesService.StopAsyncDecreaseTask();
        }

        public void Dispose()
        {
            SavePlayerState();
            _playerEconomyService?.Dispose();
            _playerRewardsByLevelService?.Dispose();
        }
    }
}