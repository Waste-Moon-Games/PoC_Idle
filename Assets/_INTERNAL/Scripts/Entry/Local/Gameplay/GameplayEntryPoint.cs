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
            navigationModel = new();

            CreateModels(out MainGameModel mainGameModel, out PlayerInfoModel playerInfoModel, container);
            CreateViewModels(out MainGameViewModel mainGameViewModel, out PlayerInfoViewModel playerInfoViewModel);
            CreateViews(out PlayerInfoView playerInfoView, out MainGameView mainGameView);

            var economyModel = container.Resolve<GameWorldState>().PlayerState.EconomyService;
            playerInfoModel.BindModel(economyModel);

            mainGameViewModel.BindModel(mainGameModel);
            playerInfoViewModel.BindModel(playerInfoModel);

            mainGameView.BindViewModel(mainGameViewModel);
            CreateAnimationServices(out ClickAnimationsService clickAnimationsService, mainGameView.ClickableZone.transform);
            mainGameView.BindAnimationService(clickAnimationsService);

            playerInfoView.BindFormatter(new());
            playerInfoView.BindViewModel(playerInfoViewModel);
            playerInfoView.BindAnimationService(clickAnimationsService);
        }

        private void CreateModels(out MainGameModel mainGameModel, out PlayerInfoModel playerInfoModel, in DIContainer container)
        {
            var playerState = container.Resolve<GameWorldState>().PlayerState;

            mainGameModel = new MainGameModel(playerState);
            playerInfoModel = new PlayerInfoModel();
        }

        private void CreateViewModels(out MainGameViewModel mainGameViewModel, out PlayerInfoViewModel playerInfoViewModel)
        {
            mainGameViewModel = new();
            playerInfoViewModel = new();
        }

        private void CreateViews(out PlayerInfoView playerInfoView, out MainGameView mainGameView)
        {
            _loader.LoadPlayableViews(out MainGameView mView, out PlayerInfoView pView);

            playerInfoView = pView;
            mainGameView = mView;
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