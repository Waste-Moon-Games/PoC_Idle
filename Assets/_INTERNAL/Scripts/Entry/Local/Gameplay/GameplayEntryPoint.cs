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
            var gameWorldState = container.Resolve<GameWorldState>();

            navigationModel = new(gameWorldState.AudioSystemService);

            CreateModels(
                out MainGameModel mainGameModel,
                out EconomyPlayerInfoModel economyPlayerInfoModel,
                out PlayerStatsModel playerStatsModel,
                out OfflineIncomeModel offlineIncomeModel,
                out SettingsModel settingsModel,
                gameWorldState);
            CreateViewModels(
                out MainGameViewModel mainGameViewModel,
                out EconomyPlayerInfoViewModel economyPlayerInfoViewModel,
                out PlayerStatsViewModel playerStatsViewModel,
                out OfflineIncomeViewModel offlineIncomeViewModel,
                out SettingsViewModel settingsViewModel);
            CreateViews(
                out EconomyPlayerInfoView economyPlayerInfoView,
                out MainGameView mainGameView,
                out PlayerStatsView playerStatsView,
                out OfflineIncomeView offlineIncomeView,
                out SettingsView settingsView);

            CreateLocalRewardsSystem(gameWorldState, mainGameView);
            CreateLocalRewardedAdsSystem(container, gameWorldState, mainGameView);

            topRootView.AttachView(economyPlayerInfoView.transform);
            topRootView.AttachView(playerStatsView.transform);

            economyPlayerInfoModel.BindModel(gameWorldState.PlayerState.EconomyService, gameWorldState.PlayerState.PlayerRewardedBonusesService);
            playerStatsModel.BindModel(gameWorldState.PlayerState);
            settingsModel.BindNavigationActions(navigationModel.Actions.Where(action => action == MainMenuEvents.SettingsClicked));

            mainGameViewModel.BindModel(mainGameModel);
            economyPlayerInfoViewModel.BindModel(economyPlayerInfoModel);
            playerStatsViewModel.BindModel(playerStatsModel);
            offlineIncomeViewModel.BindModel(offlineIncomeModel);
            settingsViewModel.BindModel(settingsModel);

            mainGameView.BindViewModel(mainGameViewModel);
            CreateAnimationServices(out ClickAnimationsService clickAnimationsService, mainGameView.ClickableZone.transform);
            mainGameView.BindAnimationService(clickAnimationsService);

            economyPlayerInfoView.BindFormatter(new());
            economyPlayerInfoView.BindViewModel(economyPlayerInfoViewModel);
            economyPlayerInfoView.BindAnimationService(clickAnimationsService);

            playerStatsView.BindViewModel(playerStatsViewModel);
            offlineIncomeView.BindViewModel(offlineIncomeViewModel);
            settingsView.BindViewModel(settingsViewModel);

            mainGameView.AttachView(offlineIncomeView.gameObject);
            mainGameView.AttachView(settingsView.gameObject);

            gameWorldState.AudioSystemService.StartPlayMainThemeMusic();

#if UNITY_WEBGL
            YG2.GameplayStart();
#endif
        }

        private void CreateLocalRewardsSystem(in GameWorldState gameWorldState, in MainGameView mainGameView)
        {
            var rewardsService = gameWorldState.PlayerState.RewardsService;
            var audioSystemService = gameWorldState.AudioSystemService;

            RewardsSystemModel model = new(rewardsService);
            model.InitRewards();

            RewardsSystemViewModel viewModel = new();
            viewModel.BindModel(model);

            RewardsSystemView view = _loader.LoadRewardSystemView();
            view.BindViewModel(viewModel);
            view.BindAudioSystemService(audioSystemService);

            mainGameView.AttachView(view.gameObject);
        }

        private void CreateLocalRewardedAdsSystem(in DIContainer container, in GameWorldState gameWorldState, in MainGameView mainGameView)
        {
            var adsSystem = container.Resolve<AdsSystemContext>();
            var bonusesService = gameWorldState.PlayerState.PlayerRewardedBonusesService;
            var localizationService = gameWorldState.LocalizationService;
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
            out SettingsModel settingsModel,
            in GameWorldState gameWorldState)
        {
            var playerState = gameWorldState.PlayerState;
            var offlineIncomeLocalizationConfig = _loader.LoadOfflineLocalizationConfig();

            mainGameModel = new MainGameModel(playerState, gameWorldState.AudioSystemService);
            playerInfoModel = new EconomyPlayerInfoModel();
            playerStatsModel = new PlayerStatsModel();
            offlineIncomeModel = new(
                playerState.PlayerOfflineCalculator.OfflineIncomeCalculatedSignal,
                playerState.PlayerOfflineCalculator.OfflineHoursCalculatedSignal,
                playerState.OfflineIncomeReceiveService,
                gameWorldState.LocalizationService,
                new(),
                offlineIncomeLocalizationConfig);

            settingsModel = new(gameWorldState.AudioSystemService);
        }

        private void CreateViewModels(
            out MainGameViewModel mainGameViewModel,
            out EconomyPlayerInfoViewModel playerInfoViewModel,
            out PlayerStatsViewModel playerStatsViewModel,
            out OfflineIncomeViewModel offlineIncomeViewModel,
            out SettingsViewModel settingsViewModel)
        {
            mainGameViewModel = new();
            playerInfoViewModel = new();
            playerStatsViewModel = new();
            offlineIncomeViewModel = new();
            settingsViewModel = new();
        }

        private void CreateViews(
            out EconomyPlayerInfoView playerInfoView,
            out MainGameView mainGameView,
            out PlayerStatsView playerStatsView,
            out OfflineIncomeView offlineIncomeView,
            out SettingsView settingsView)
        {
            _loader.LoadPlayableViews(out MainGameView mView, out EconomyPlayerInfoView pView, out PlayerStatsView pStatsView);

            playerInfoView = pView;
            mainGameView = mView;
            playerStatsView = pStatsView;
            offlineIncomeView = _loader.LoadOfflineIncomeView();
            settingsView = _loader.LoadSettignsView();
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