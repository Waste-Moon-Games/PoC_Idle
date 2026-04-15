using Core.AdsSystem;
using Core.GlobalGameState;

using R3;

using SO.AnimationConfigs;

using UI.GameplayMenu.Animations;
using UI.GameplayMenu.Models;
using UI.GameplayMenu.Models.BonusesFromRewardAd;
using UI.GameplayMenu.ViewModels;
using UI.GameplayMenu.ViewModels.BonusesFromRewardAd;
using UI.GameplayMenu.Views;
using UI.GameplayMenu.Views.BonusesFromRewardAd;

using UnityEngine;

using Utils.DI;
#if UNITY_WEBGL
using YG;
#endif

namespace Entry.Local.Gameplay
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        private readonly GameplayResourceLoader _loader = new();

        public Observable<MainMenuEvents> Run(DIContainer container)
        {
            _loader.LoadMainViews(out NavigationButtonsView navigationButtonsView);

            CreateScene(container, out NavigationButtonsModel navigationModel);

            var navigationViewModel = new NavigationButtonsViewModel();

            navigationViewModel.BindModel(navigationModel);
            navigationButtonsView.BindViewModel(navigationViewModel);

            return navigationModel.Actions.Where(action => action == MainMenuEvents.ShopClicked);
        }

        private void CreateScene(DIContainer container, out NavigationButtonsModel navigationModel)
        {
            _loader.LoadRootViews(out UIRootTopBlockView topRootView);

            navigationModel = new();

            CreateModels(
                out MainGameModel mainGameModel,
                out EconomyPlayerInfoModel economyPlayerInfoModel,
                out PlayerStatsModel playerStatsModel,
                out OfflineIncomeModel offlineIncomeModel,
                container);
            CreateViewModels(
                out MainGameViewModel mainGameViewModel,
                out EconomyPlayerInfoViewModel economyPlayerInfoViewModel,
                out PlayerStatsViewModel playerStatsViewModel,
                out OfflineIncomeViewModel offlineIncomeViewModel);
            CreateViews(
                out EconomyPlayerInfoView economyPlayerInfoView,
                out MainGameView mainGameView,
                out PlayerStatsView playerStatsView,
                out OfflineIncomeView offlineIncomeView);

            CreateLocalRewardsSystem(container, mainGameView);
            CreateLocalRewardedAdsSystem(container, mainGameView);

            topRootView.AttachView(economyPlayerInfoView.transform);
            topRootView.AttachView(playerStatsView.transform);

            var playerState = container.Resolve<GameWorldState>().PlayerState;

            economyPlayerInfoModel.BindModel(playerState.EconomyService, playerState.PlayerRewardedBonusesService);
            playerStatsModel.BindModel(playerState);

            mainGameViewModel.BindModel(mainGameModel);
            economyPlayerInfoViewModel.BindModel(economyPlayerInfoModel);
            playerStatsViewModel.BindModel(playerStatsModel);
            offlineIncomeViewModel.BindModel(offlineIncomeModel);

            mainGameView.BindViewModel(mainGameViewModel);
            CreateAnimationServices(out ClickAnimationsService clickAnimationsService, mainGameView.ClickableZone.transform);
            mainGameView.BindAnimationService(clickAnimationsService);

            economyPlayerInfoView.BindFormatter(new());
            economyPlayerInfoView.BindViewModel(economyPlayerInfoViewModel);
            economyPlayerInfoView.BindAnimationService(clickAnimationsService);

            playerStatsView.BindViewModel(playerStatsViewModel);
            offlineIncomeView.BindViewModel(offlineIncomeViewModel);

            mainGameView.AttachView(offlineIncomeView.gameObject);
            Debug.Log("[Gameplay Entry Point] Gameplay Scene created");

#if UNITY_WEBGL
            YG2.GameplayStart();
#endif
        }

        private void CreateLocalRewardsSystem(in DIContainer container, in MainGameView mainGameView)
        {
            var rewardsService = container.Resolve<GameWorldState>().PlayerState.RewardsService;

            RewardsSystemModel model = new(rewardsService);
            model.InitRewards();

            RewardsSystemViewModel viewModel = new();
            viewModel.BindModel(model);

            RewardsSystemView view = _loader.LoadRewardSystemView();
            view.BindViewModel(viewModel);

            mainGameView.AttachView(view.gameObject);
        }

        private void CreateLocalRewardedAdsSystem(in DIContainer container, in MainGameView mainGameView)
        {
            var adsSystem = container.Resolve<AdsSystemContext>();
            var bonusesService = container.Resolve<GameWorldState>().PlayerState.PlayerRewardedBonusesService;
            var localizationService = container.Resolve<GameWorldState>().LocalizationService;
            var rewardedBonusesConfig = _loader.LoadRewardedBonusesConfig();

            PlayerRewardedBonusesModel model = new(adsSystem, bonusesService);
            model.CreateBonusItemModels(rewardedBonusesConfig.Items, localizationService.CurrentLanguage);

            PlayerRewardedBonusesViewModel viewModel = new();
            viewModel.BindModel(model);

            PlayerRewardedBonusesView view = _loader.LoadPlayerRewardedBonusesService();
            view.BindViewModel(viewModel);

            mainGameView.AttachView(view.gameObject);
        }

        private void CreateModels(
            out MainGameModel mainGameModel,
            out EconomyPlayerInfoModel playerInfoModel,
            out PlayerStatsModel playerStatsModel,
            out OfflineIncomeModel offlineIncomeModel,
            in DIContainer container)
        {
            var playerState = container.Resolve<GameWorldState>().PlayerState;

            mainGameModel = new MainGameModel(playerState);
            playerInfoModel = new EconomyPlayerInfoModel();
            playerStatsModel = new PlayerStatsModel();
            offlineIncomeModel = new(
                playerState.PlayerOfflineCalculator.OfflineIncomeCalculatedSignal,
                playerState.PlayerOfflineCalculator.OfflineHoursCalculatedSignal,
                playerState.OfflineIncomeReceiveService);
        }

        private void CreateViewModels(
            out MainGameViewModel mainGameViewModel,
            out EconomyPlayerInfoViewModel playerInfoViewModel,
            out PlayerStatsViewModel playerStatsViewModel,
            out OfflineIncomeViewModel offlineIncomeViewModel)
        {
            mainGameViewModel = new();
            playerInfoViewModel = new();
            playerStatsViewModel = new();
            offlineIncomeViewModel = new(new());
        }

        private void CreateViews(
            out EconomyPlayerInfoView playerInfoView,
            out MainGameView mainGameView,
            out PlayerStatsView playerStatsView,
            out OfflineIncomeView offlineIncomeView)
        {
            _loader.LoadPlayableViews(out MainGameView mView, out EconomyPlayerInfoView pView, out PlayerStatsView pStatsView);

            playerInfoView = pView;
            mainGameView = mView;
            playerStatsView = pStatsView;
            offlineIncomeView = _loader.LoadOfflineIncomeView();
        }

        private void CreateAnimationServices(out ClickAnimationsService clickAnimationsService, in Transform target)
        {
            var animationsConfig = Resources.Load<ClickAnimationsConfig>("Configs/Animations/ClickAnimationsConfig");
            clickAnimationsService = new
                (target,
                animationsConfig.TargetScale,
                animationsConfig.DefaultScale,
                animationsConfig.Prefab,
                animationsConfig.InitPoolCount);
        }
    }
}