using Core.GlobalGameState;
using R3;
using SO.AnimationConfigs;
using UI.GameplayMenu.Animations;
using UI.GameplayMenu.Models;
using UI.GameplayMenu.ViewModels;
using UI.GameplayMenu.Views;
using UnityEngine;
using Utils.DI;

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
                container);
            CreateViewModels(
                out MainGameViewModel mainGameViewModel,
                out EconomyPlayerInfoViewModel economyPlayerInfoViewModel,
                out PlayerStatsViewModel playerStatsViewModel);
            CreateViews(
                out EconomyPlayerInfoView economyPlayerInfoView,
                out MainGameView mainGameView,
                out PlayerStatsView playerStatsView);

            topRootView.AttachView(economyPlayerInfoView.transform);
            topRootView.AttachView(playerStatsView.transform);

            var economyModel = container.Resolve<GameWorldState>().PlayerState.EconomyService;
            var playerState = container.Resolve<GameWorldState>().PlayerState;

            economyPlayerInfoModel.BindModel(economyModel);
            playerStatsModel.BindModel(playerState);

            mainGameViewModel.BindModel(mainGameModel);
            economyPlayerInfoViewModel.BindModel(economyPlayerInfoModel);
            playerStatsViewModel.BindModel(playerStatsModel);

            mainGameView.BindViewModel(mainGameViewModel);
            CreateAnimationServices(out ClickAnimationsService clickAnimationsService, mainGameView.ClickableZone.transform);
            mainGameView.BindAnimationService(clickAnimationsService);

            economyPlayerInfoView.BindFormatter(new());
            economyPlayerInfoView.BindViewModel(economyPlayerInfoViewModel);
            economyPlayerInfoView.BindAnimationService(clickAnimationsService);

            playerStatsView.BindViewModel(playerStatsViewModel);
        }

        private void CreateModels(
            out MainGameModel mainGameModel,
            out EconomyPlayerInfoModel playerInfoModel,
            out PlayerStatsModel playerStatsModel,
            in DIContainer container)
        {
            var playerState = container.Resolve<GameWorldState>().PlayerState;

            mainGameModel = new MainGameModel(playerState);
            playerInfoModel = new EconomyPlayerInfoModel();
            playerStatsModel = new PlayerStatsModel();
        }

        private void CreateViewModels(
            out MainGameViewModel mainGameViewModel,
            out EconomyPlayerInfoViewModel playerInfoViewModel,
            out PlayerStatsViewModel playerStatsViewModel)
        {
            mainGameViewModel = new();
            playerInfoViewModel = new();
            playerStatsViewModel = new();
        }

        private void CreateViews(
            out EconomyPlayerInfoView playerInfoView,
            out MainGameView mainGameView,
            out PlayerStatsView playerStatsView)
        {
            _loader.LoadPlayableViews(out MainGameView mView, out EconomyPlayerInfoView pView, out PlayerStatsView pStatsView);

            playerInfoView = pView;
            mainGameView = mView;
            playerStatsView = pStatsView;
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