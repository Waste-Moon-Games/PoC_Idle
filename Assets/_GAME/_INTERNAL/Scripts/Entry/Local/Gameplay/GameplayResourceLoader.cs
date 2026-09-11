using Common.MVVM;
using Core.AddressablesLoadSystem;

using Cysharp.Threading.Tasks;

using SO;
using SO.AdsConfigs;
using SO.PlayerConfigs;

using UI.GameplayMenu.Views;
using UI.GameplayMenu.Views.BonusesFromRewardAd;
using UI.GameplayMenu.Views.Settings;
using UnityEngine;

using Utils.CustomResourceLoader;

namespace Entry.Local.Gameplay
{
    public class GameplayResourceLoader : IGameplayResourceLoader
    {
        // TODO: remove config
        private GameResourcePathsConfig _resourcePathsConfig;

        #region Legacy API
        // Legacy API

        public void BindResourcePathsConfig(GameResourcePathsConfig config) => _resourcePathsConfig = config;
        
        public UIRootTopBlockView LoadTopRootView()
        {
            var topRootViewPrefab = ResourceLoader.LoadOrThrow<UIRootTopBlockView>(GameplayResourcePathKeys.UIRootTopBlockViewKey);

            var topRootView = Object.Instantiate(topRootViewPrefab);
            return topRootView;
        }

        public NavigationButtonsView LoadNavigationView()
        {
            var navigationButtonsViewPrefab = ResourceLoader.LoadOrThrow<NavigationButtonsView>(GameplayResourcePathKeys.NavigationButtonsViewKey);

            var navigationButtonsView = Object.Instantiate(navigationButtonsViewPrefab);
            return navigationButtonsView;
        }

        public RewardsSystemView LoadRewardsSystemView()
        {
            var rewardSystemViewPrefab = ResourceLoader.LoadOrThrow<RewardsSystemView>(GameplayResourcePathKeys.RewardsViewHolderKey);

            var rewardSystemView = Object.Instantiate(rewardSystemViewPrefab);
            return rewardSystemView;
        }

        public PlayerRewardedBonusesView LoadPlayerRewardedBonusesView()
        {
            var path = _resourcePathsConfig.GetPathByKeyWord(GameplayResourcePathKeys.RewardedBonusesViewHolderKey);
            var rewardedBonusesViewPrefab = ResourceLoader.LoadOrThrow<PlayerRewardedBonusesView>(path);

            var rewardedBonusesView = Object.Instantiate(rewardedBonusesViewPrefab);
            return rewardedBonusesView;
        }

        public void LoadPlayableViews(out MainGameView mainGameView, out EconomyPlayerInfoView economyPlayerInfoView, out PlayerStatsView playerStatsView)
        {
            var mainGamePath = _resourcePathsConfig.GetPathByKeyWord(GameplayResourcePathKeys.UIRootViewKey);
            var economyViewPath = _resourcePathsConfig.GetPathByKeyWord(GameplayResourcePathKeys.EconomyPlayerInfoKey);
            var playerStateViewPath = _resourcePathsConfig.GetPathByKeyWord(GameplayResourcePathKeys.PlayerStatsInfoViewKey);

            var mainGameViewPrefab = ResourceLoader.LoadOrThrow<MainGameView>(mainGamePath);
            var economyPlayerInfoViewPrefab = ResourceLoader.LoadOrThrow<EconomyPlayerInfoView>(economyViewPath);
            var playerStatsViewPrefab = ResourceLoader.LoadOrThrow<PlayerStatsView>(playerStateViewPath);

            mainGameView = Object.Instantiate(mainGameViewPrefab);
            economyPlayerInfoView = Object.Instantiate(economyPlayerInfoViewPrefab);
            playerStatsView = Object.Instantiate(playerStatsViewPrefab);
        }

        public OfflineIncomeView LoadOfflineIncomeView()
        {
            var path = _resourcePathsConfig.GetPathByKeyWord(GameplayResourcePathKeys.OfflineIncomeViewKey);
            var prefab = ResourceLoader.LoadOrThrow<OfflineIncomeView>(path);

            var offlineIncomeView = Object.Instantiate(prefab);

            return offlineIncomeView;
        }

        public SettingsView LoadSettingsView()
        {
            var path = _resourcePathsConfig.GetPathByKeyWord(GameplayResourcePathKeys.SettingsPanelViewKey);
            var prefab = Resources.Load<SettingsView>(path);

            var settingsView = Object.Instantiate(prefab);

            return settingsView;
        }

        public RewardAdsConfig LoadRewardAdsConfig()
        {
            var path = _resourcePathsConfig.GetPathByKeyWord(GameplayResourcePathKeys.RewardAdsConfigKey);
            var result = ResourceLoader.LoadOrThrow<RewardAdsConfig>(path);
            return result;
        }

        public OfflineIncomeLocalizationConfig LoadOfflineIncomeLocalizationConfig()
        {
            var path = _resourcePathsConfig.GetPathByKeyWord(GameplayResourcePathKeys.OfflineIncomeLocalizationConfigKey);
            var result = ResourceLoader.LoadOrThrow<OfflineIncomeLocalizationConfig>(path);
            return result;
        }
        #endregion

        #region Addressables API
        // New addressables API
        public UniTask<UIRootTopBlockView> LoadRootViewAsync()
        {
            throw new System.NotImplementedException();
        }

        public UniTask<NavigationButtonsView> LoadNavigationViewAsync()
        {
            throw new System.NotImplementedException();
        }

        public UniTask<RewardsSystemView> LoadRewardsSystemViewAsync()
        {
            throw new System.NotImplementedException();
        }

        public UniTask<PlayerRewardedBonusesView> LoadPlayerRewardedBonusesViewAsync()
        {
            throw new System.NotImplementedException();
        }

        public UniTask<(MainGameView, EconomyPlayerInfoView, PlayerStatsView)> LoadPlayableViewAsync()
        {
            throw new System.NotImplementedException();
        }

        public UniTask<OfflineIncomeView> LoadOfflineIncomeViewAsync()
        {
            throw new System.NotImplementedException();
        }

        public UniTask<SettingsView> LoadSettignsViewAsync()
        {
            throw new System.NotImplementedException();
        }

        public UniTask<RewardAdsConfig> LoadRewardsAdsConfigAsync()
        {
            throw new System.NotImplementedException();
        }

        public UniTask<OfflineIncomeLocalizationConfig> LoadOfflineIncomeLocalizationConfigAsync()
        {
            throw new System.NotImplementedException();
        }

        public async UniTask<T> LoadViewEntity<T>(string path) where T: Object, IView
        {
            var entityPrefab = await Resources.LoadAsync<T>(path);
            if(entityPrefab == null)
                throw new System.ArgumentNullException(nameof(entityPrefab), $"Entity not found by path: {path}");

            Object entity = Object.Instantiate(entityPrefab);
            return (T)entity;
        }
        #endregion
    }
}