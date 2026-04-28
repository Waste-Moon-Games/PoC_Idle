using SO.AdsConfigs;
using SO.PlayerConfigs;
using UI.GameplayMenu.Views;
using UI.GameplayMenu.Views.BonusesFromRewardAd;
using UnityEngine;

namespace Entry.Local.Gameplay
{
    public class GameplayResourceLoader
    {
        public void LoadRootViews(out UIRootTopBlockView topRootView)
        {
            var topRootViewPrefab = Resources.Load<UIRootTopBlockView>("UI/GameplayMenu/UIBlocks/Root/UIRootTopBlockView");

            topRootView = Object.Instantiate(topRootViewPrefab);
        }

        public void LoadMainViews(out NavigationButtonsView navigationButtonsView)
        {
            var navigationButtonsViewPrefab = Resources.Load<NavigationButtonsView>("UI/GameplayMenu/UIBlocks/UIButtonsView");

            navigationButtonsView = Object.Instantiate(navigationButtonsViewPrefab);
        }

        public RewardsSystemView LoadRewardSystemView()
        {
            var rewardSystemViewPrefab = Resources.Load<RewardsSystemView>("UI/GameplayMenu/UIBlocks/RewardsViewHolder");

            return Object.Instantiate(rewardSystemViewPrefab);
        }

        public PlayerRewardedBonusesView LoadPlayerRewardedBonusesService()
        {
            var rewardedBonusesViewPrefab = Resources.Load<PlayerRewardedBonusesView>("UI/GameplayMenu/UIBlocks/RewardedBonusesViewHolder");
            
            return Object.Instantiate(rewardedBonusesViewPrefab);
        }

        public void LoadPlayableViews(out MainGameView mainGameView, out EconomyPlayerInfoView economyPlayerInfoView, out PlayerStatsView playerStatsView)
        {
            var mainGameViewPrefab = Resources.Load<MainGameView>("UI/GameplayMenu/UIBlocks/UIRootView");
            var economyPlayerInfoViewPrefab = Resources.Load<EconomyPlayerInfoView>("UI/GameplayMenu/UIBlocks/EconomyPlayerInfo");
            var playerStatsViewPrefab = Resources.Load<PlayerStatsView>("UI/GameplayMenu/UIBlocks/PlayerStatsInfoView");

            mainGameView = Object.Instantiate(mainGameViewPrefab);
            economyPlayerInfoView = Object.Instantiate(economyPlayerInfoViewPrefab);
            playerStatsView = Object.Instantiate(playerStatsViewPrefab);
        }

        public OfflineIncomeView LoadOfflineIncomeView()
        {
            var prefab = Resources.Load<OfflineIncomeView>("UI/GameplayMenu/SmallPrefabs/OfflineIncomeView");
            if(prefab == null)
            {
                Debug.LogWarning("[Gameplay Resource Loader] Offline Income View is null!");
                return null;
            }

            var offlineIncomeView = Object.Instantiate(prefab);

            return offlineIncomeView;
        }

        public SettingsView LoadSettignsView()
        {
            var prefab = Resources.Load<SettingsView>("UI/GameplayMenu/UIBlocks/SettingsPanelView");

            var settingsView = Object.Instantiate(prefab);

            return settingsView;
        }

        public RewardAdsConfig LoadRewardedBonusesConfig()
        {
            var result = Resources.Load<RewardAdsConfig>("Configs/Ads/RewardAdsConfig");
            return result;
        }

        public OfflineIncomeLocalizationConfig LoadOfflineLocalizationConfig()
        {
            var result = Resources.Load<OfflineIncomeLocalizationConfig>("Configs/Player/OfflineIncomeLocalizationConfig");
            return result;
        }
    }
}