using Core.AdsSystem;
using Core.GlobalGameState;
using Core.SaveSystemBase;

using Cysharp.Threading.Tasks;
using SO.AdsConfigs;
using UI.Common;

using UnityEngine;

using Utils.DI;
using Utils.SceneLoader;

#if UNITY_WEBGL
using YG;
#endif

namespace Entry.Global
{
    public class GameEntry
    {
        private static GameEntry _instance;

        private readonly DIContainer _rootContainer = new();

        private readonly SceneNavigatorService _sceneNavigatorService;
        private readonly UILoadingView _loadingView;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static async UniTask AutoStart()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            _instance = new GameEntry();
            await _instance.Run();

            Application.quitting += HandleApplicationQuit;
        }

        private GameEntry()
        {
            var loadingScreenPrefab = Resources.Load<UILoadingView>("UI/Common/UILoadingView");

            _loadingView = Object.Instantiate(loadingScreenPrefab);

            RegisterGlobalServices();

            var scl = _rootContainer.Resolve<SceneLoaderService>();
            _sceneNavigatorService = new(scl, _rootContainer);
        }

        private void RegisterGlobalServices()
        {
            var adRatesConfig = Resources.Load<AdRatesConfig>("Configs/Ads/AdRatesConfig");
            _rootContainer.RegisterInstance(_loadingView);

            var loadingScreen = _rootContainer.Resolve<UILoadingView>();
            _rootContainer.RegisterFactory(slc => new SceneLoaderService(loadingScreen)).AsSingle();

            _rootContainer.RegisterFactory(ssc => new SaveSystemContext(SaveSystemStrategyFactory.CreateStrategy())).AsSingle();
            _rootContainer.RegisterFactory(ads => new AdsSystemContex(AdsStrategyFactory.CreateStrategy(), adRatesConfig.InterstitialAdShowChance)).AsSingle();

            var saveSystemContex = _rootContainer.Resolve<SaveSystemContext>();
            _rootContainer.RegisterFactory(gws => new GameWorldState(saveSystemContex)).AsSingle();
        }

        private async UniTask Run()
        {
            var gameWorldState = _rootContainer.Resolve<GameWorldState>();
            await gameWorldState.InitPlayerState();
            gameWorldState.StartAsyncOperations();

            _sceneNavigatorService.Start();
        }

        private static void HandleApplicationQuit()
        {
            _instance._rootContainer.Resolve<GameWorldState>().Dispose();
            _instance._sceneNavigatorService.Dispose();
            _instance._rootContainer.Dispose();

#if UNITY_WEBGL
            YG2.GameplayStop();
#endif
        }
    }
}