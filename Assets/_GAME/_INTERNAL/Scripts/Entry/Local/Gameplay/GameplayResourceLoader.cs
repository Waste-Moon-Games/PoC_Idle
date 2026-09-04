using Core.AddressablesLoadSystem;

using Cysharp.Threading.Tasks;

using SO.AdsConfigs;
using SO.PlayerConfigs;

using UI.GameplayMenu.Views;
using UI.GameplayMenu.Views.BonusesFromRewardAd;

using UnityEngine;

using Utils.CustomResourceLoader;

namespace Entry.Local.Gameplay
{
    public class GameplayResourceLoader : IGameplayResourceLoader
    {
        #region Legacy API
        // Legacy API
        public UIRootTopBlockView LoadTopRootView()
        {
            var topRootViewPrefab = ResourceLoader.LoadOrThrow<UIRootTopBlockView>(GameplayResourcePaths.UIRootTopBlockView);

            var topRootView = Object.Instantiate(topRootViewPrefab);
            return topRootView;
        }

        public NavigationButtonsView LoadNavigationView()
        {
            var navigationButtonsViewPrefab = ResourceLoader.LoadOrThrow<NavigationButtonsView>(GameplayResourcePaths.NavigationButtonsView);

            var navigationButtonsView = Object.Instantiate(navigationButtonsViewPrefab);
            return navigationButtonsView;
        }

        public RewardsSystemView LoadRewardsSystemView()
        {
            var rewardSystemViewPrefab = ResourceLoader.LoadOrThrow<RewardsSystemView>(GameplayResourcePaths.RewardsViewHolder);

            var rewardSystemView = Object.Instantiate(rewardSystemViewPrefab);
            return rewardSystemView;
        }

        public PlayerRewardedBonusesView LoadPlayerRewardedBonusesView()
        {
            var rewardedBonusesViewPrefab = ResourceLoader.LoadOrThrow<PlayerRewardedBonusesView>(GameplayResourcePaths.RewardedBonusesViewHolder);

            var rewardedBonusesView = Object.Instantiate(rewardedBonusesViewPrefab);
            return rewardedBonusesView;
        }

        public void LoadPlayableViews(out MainGameView mainGameView, out EconomyPlayerInfoView economyPlayerInfoView, out PlayerStatsView playerStatsView)
        {
            var mainGameViewPrefab = ResourceLoader.LoadOrThrow<MainGameView>(GameplayResourcePaths.UIRootView);
            var economyPlayerInfoViewPrefab = ResourceLoader.LoadOrThrow<EconomyPlayerInfoView>(GameplayResourcePaths.EconomyPlayerInfo);
            var playerStatsViewPrefab = ResourceLoader.LoadOrThrow<PlayerStatsView>(GameplayResourcePaths.PlayerStatsInfoView);

            mainGameView = Object.Instantiate(mainGameViewPrefab);
            economyPlayerInfoView = Object.Instantiate(economyPlayerInfoViewPrefab);
            playerStatsView = Object.Instantiate(playerStatsViewPrefab);
        }

        public OfflineIncomeView LoadOfflineIncomeView()
        {
            var prefab = ResourceLoader.LoadOrThrow<OfflineIncomeView>(GameplayResourcePaths.OfflineIncomeView);

            var offlineIncomeView = Object.Instantiate(prefab);

            return offlineIncomeView;
        }

        public SettingsView LoadSettingsView()
        {
            var prefab = Resources.Load<SettingsView>(GameplayResourcePaths.SettingsPanelView);

            var settingsView = Object.Instantiate(prefab);

            return settingsView;
        }

        public RewardAdsConfig LoadRewardAdsConfig()
        {
            var result = ResourceLoader.LoadOrThrow<RewardAdsConfig>(GameplayResourcePaths.RewardAdsConfig);
            return result;
        }

        public OfflineIncomeLocalizationConfig LoadOfflineIncomeLocalizationConfig()
        {
            var result = ResourceLoader.LoadOrThrow<OfflineIncomeLocalizationConfig>(GameplayResourcePaths.OfflineIncomeLocalizationConfig);
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
        #endregion
    }
}