using Core.GlobalGameState;
using UI.Common;
using UnityEngine;
using Utils.DI;
using Utils.ModCoroutines;
using Utils.SceneLoader;

namespace Entry.Global
{
    public class GameEntry
    {
        private static GameEntry _instance;

        private readonly DIContainer _rootContainer = new();

        private readonly SceneNavigatorService _sceneNavigatorService;
        private readonly UILoadingView _loadingView;
        private readonly Coroutines _coroutines;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutoStart()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            _instance = new GameEntry();
            _instance.Run();

            Application.quitting += HandleApplicationQuit;
        }

        private GameEntry()
        {
            var loadingScreenPrefab = Resources.Load<UILoadingView>("UI/Common/UILoadingView");
            var coroutinesPrefab = Resources.Load<Coroutines>("Other/Utils/[COROUTINES]");

            _loadingView = Object.Instantiate(loadingScreenPrefab);
            _coroutines = Object.Instantiate(coroutinesPrefab);

            RegisterGlobalServices();

            var scl = _rootContainer.Resolve<SceneLoaderService>();
            _sceneNavigatorService = new(scl, _rootContainer);
        }

        private void RegisterGlobalServices()
        {
            _rootContainer.RegisterInstance(_loadingView);
            _rootContainer.RegisterInstance(_coroutines);

            var coroutines = _rootContainer.Resolve<Coroutines>();
            var loadingScreen = _rootContainer.Resolve<UILoadingView>();
            _rootContainer.RegisterFactory(slc => new SceneLoaderService(loadingScreen, coroutines)).AsSingle();

            _rootContainer.RegisterFactory(gws => new GameWorldState(coroutines)).AsSingle();
        }

        private void Run()
        {
            _sceneNavigatorService.Start();
        }

        private static void HandleApplicationQuit()
        {
            _instance._sceneNavigatorService.Dispose();
            _instance._rootContainer.Dispose();
        }
    }
}