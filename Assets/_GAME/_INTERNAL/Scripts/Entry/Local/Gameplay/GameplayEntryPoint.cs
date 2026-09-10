using Core.AddressablesLoadSystem;
using Core.AdsSystem;
using Core.GlobalGameState;
using Cysharp.Threading.Tasks;
using R3;

using RuStore;
using RuStore.Review;
using SO;
using SO.AnimationConfigs;
using System.Collections;
using System.Threading.Tasks;
using UI.GameplayMenu.Animations;
using UI.GameplayMenu.Models;
using UI.GameplayMenu.Models.BonusesFromRewardAd;
using UI.GameplayMenu.ViewModels;
using UI.GameplayMenu.ViewModels.BonusesFromRewardAd;
using UI.GameplayMenu.Views;
using UI.GameplayMenu.Views.BonusesFromRewardAd;
using UI.GameplayMenu.Views.Settings;
using UnityEngine;

using Utils.CustomResourceLoader;
using Utils.DI;
#if UNITY_WEBGL
using YG;
#endif

namespace Entry.Local.Gameplay
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        [SerializeField] private GameResourcePathsConfig _resourcePathsConfig;

        private const float ReviewDelaySeconds = 5f * 60f;

        private readonly GameplayResourceLoader _loader = new();
        private ReviewScreen _reviewScreen;
        private MainGameView _mainGameView;

        private DIContainer _container;

        private float _activeGameplayTime;
        private bool _reviewFlowPrepared;
        private bool _reviewWindowShown;

        public async UniTask<Observable<MainMenuEvents>> Run(DIContainer container)
        {
            string path = _resourcePathsConfig.GetPathByKeyWord(GameplayResourcePathKeys.NavigationButtonsViewKey);
            _loader.BindResourcePathsConfig(_resourcePathsConfig);

            NavigationButtonsView view = await _loader.LoadViewEntity<NavigationButtonsView>(path);

            NavigationButtonsModel model = await CreateScene(container);

            var viewModel = new NavigationButtonsViewModel();

            viewModel.BindModel(model);
            view.BindViewModel(viewModel);

            return model.Actions.Where(action => action == MainMenuEvents.ShopClicked);
        }

        private async UniTask<NavigationButtonsModel> CreateScene(DIContainer container)
        {
            _container = container;

            string path = _resourcePathsConfig.GetPathByKeyWord(GameplayResourcePathKeys.UIRootTopBlockViewKey);
            UIRootTopBlockView topRootView = await _loader.LoadViewEntity<UIRootTopBlockView>(path);
            
            var gameWorldState = container.Resolve<GameWorldState>();

            NavigationButtonsModel navigationModel = new();

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

            _mainGameView = mainGameView;

            await CreateLocalRewardsSystem();
            await CreateLocalRewardedAdsSystem();

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

            var reviewScreenPrefab = ResourceLoader.LoadOrThrow<ReviewScreen>("UI/Common/ReviewScreen");
            _reviewScreen = Instantiate(reviewScreenPrefab);
            _reviewScreen.gameObject.SetActive(false);
            mainGameView.AttachView(_reviewScreen.gameObject);

#if UNITY_ANDROID
            RuStoreReviewManager.Instance.RequestReviewFlow(
                onFailure: (error) =>
                {
                    HandleFailureRequestReview(error);
                },
                onSuccess: HandleSuccessRequestReview);
#endif

#if UNITY_WEBGL
            YG2.GameplayStart();
#endif
#if UNITY_ANDROID
            StartCoroutine(TrackPlayerActivityForReview());
#endif

            return navigationModel;
        }

#if UNITY_ANDROID
        private IEnumerator TrackPlayerActivityForReview()
        {
            while (_reviewWindowShown == false)
            {
                if (Application.isFocused && IsPlayerActive())
                    _activeGameplayTime += Time.unscaledDeltaTime;

                TryShowReviewWindow();
                yield return null;
            }
        }

        private bool IsPlayerActive()
        {
            if (Input.touchCount > 0)
                return true;

            return Input.anyKey || Input.GetMouseButton(0);
        }

        private void TryShowReviewWindow()
        {
            if (_reviewWindowShown)
                return;

            if (_reviewFlowPrepared == false)
                return;

            if (_activeGameplayTime < ReviewDelaySeconds)
                return;

            _reviewWindowShown = true;
            _reviewScreen.gameObject.SetActive(true);
        }
#endif

        private async UniTask CreateLocalRewardsSystem()
        {
            var gameWorldState = _container.Resolve<GameWorldState>();
            var rewardsService = gameWorldState.PlayerState.RewardsService;
            var audioSystemService = gameWorldState.AudioSystemService;

            RewardsSystemModel model = new(rewardsService);
            model.InitRewards();

            RewardsSystemViewModel viewModel = new();
            viewModel.BindModel(model);

            string path = _resourcePathsConfig.GetPathByKeyWord(GameplayResourcePathKeys.RewardsViewHolderKey);
            RewardsSystemView view = await _loader.LoadViewEntity<RewardsSystemView>(path);
            view.BindViewModel(viewModel);

            _mainGameView.AttachView(view.gameObject);
        }

        private async UniTask CreateLocalRewardedAdsSystem()
        {
            var adsSystem = _container.Resolve<AdsSystemContext>();
            var gameWorldState = _container.Resolve<GameWorldState>();
            var bonusesService = gameWorldState.PlayerState.PlayerRewardedBonusesService;
            var localizationService = gameWorldState.LocalizationService;
            var rewardedBonusesConfig = _loader.LoadRewardAdsConfig();

            PlayerRewardedBonusesModel model = new(adsSystem, bonusesService);
            model.CreateBonusItemModels(rewardedBonusesConfig.Items, localizationService.CurrentLanguage);

            PlayerRewardedBonusesViewModel viewModel = new();
            viewModel.BindModel(model);

            string path = _resourcePathsConfig.GetPathByKeyWord(GameplayResourcePathKeys.RewardedBonusesViewHolderKey);
            PlayerRewardedBonusesView view = await _loader.LoadViewEntity<PlayerRewardedBonusesView>(path);
            view.BindViewModel(viewModel);

            _mainGameView.AttachView(view.gameObject);
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
            var offlineIncomeLocalizationConfig = _loader.LoadOfflineIncomeLocalizationConfig();

            mainGameModel = new MainGameModel(playerState);
            playerInfoModel = new EconomyPlayerInfoModel();
            playerStatsModel = new PlayerStatsModel();
            offlineIncomeModel = new(
                playerState.PlayerOfflineCalculator,
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
            settingsView = _loader.LoadSettingsView();
        }

        private void CreateAnimationServices(out ClickAnimationsService clickAnimationsService, in Transform target)
        {
            var animationsConfig = ResourceLoader.LoadOrThrow<ClickAnimationsConfig>("Configs/Animations/ClickAnimationsConfig");
            clickAnimationsService = new(target, animationsConfig);
        }

#if UNITY_ANDROID
        private void HandleSuccessRequestReview()
        {
            _reviewFlowPrepared = true;
            TryShowReviewWindow();
        }

        private void HandleFailureRequestReview(RuStoreError error) { }
#endif
    }
}