using Common.MVVM;
using Cysharp.Threading.Tasks;
using SO.AdsConfigs;
using SO.PlayerConfigs;
using UI.GameplayMenu.Views;
using UI.GameplayMenu.Views.BonusesFromRewardAd;
using UI.GameplayMenu.Views.Settings;
using UnityEngine;

namespace Core.AddressablesLoadSystem
{
    public interface IGameplayResourceLoader
    {
        // Legacy API for soft migration
        UIRootTopBlockView LoadTopRootView();
        NavigationButtonsView LoadNavigationView();
        RewardsSystemView LoadRewardsSystemView();
        PlayerRewardedBonusesView LoadPlayerRewardedBonusesView();

        void LoadPlayableViews(out MainGameView mainGameView, out EconomyPlayerInfoView economyPlayerInfoView, out PlayerStatsView playerStatsView);
        OfflineIncomeView LoadOfflineIncomeView();
        SettingsView LoadSettingsView();

        RewardAdsConfig LoadRewardAdsConfig();
        OfflineIncomeLocalizationConfig LoadOfflineIncomeLocalizationConfig();

        // Async API target for Addressables migration
        UniTask<UIRootTopBlockView> LoadRootViewAsync();
        UniTask<NavigationButtonsView> LoadNavigationViewAsync();
        UniTask<RewardsSystemView> LoadRewardsSystemViewAsync();
        UniTask<PlayerRewardedBonusesView> LoadPlayerRewardedBonusesViewAsync();

        UniTask<(MainGameView, EconomyPlayerInfoView, PlayerStatsView)> LoadPlayableViewAsync();
        UniTask<OfflineIncomeView> LoadOfflineIncomeViewAsync();
        UniTask<SettingsView> LoadSettignsViewAsync();

        UniTask<RewardAdsConfig> LoadRewardsAdsConfigAsync();
        UniTask<OfflineIncomeLocalizationConfig> LoadOfflineIncomeLocalizationConfigAsync();

        UniTask<T> LoadViewEntity<T>(string keyWord) where T: Object, IView;
    }
}