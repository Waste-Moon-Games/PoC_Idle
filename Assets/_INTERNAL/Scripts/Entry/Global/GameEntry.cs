using Core.GlobalGameState;
using Core.SaveSystemBase;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UI.Common;
using UnityEngine;
using Utils.DI;
using Utils.SceneLoader;
using YG;

namespace Entry.Global
{
    public class GameEntry
    {
        private static GameEntry _instance;

        private readonly DIContainer _rootContainer = new();

        private readonly SceneNavigatorService _sceneNavigatorService;
        private readonly UILoadingView _loadingView;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static async Task AutoStart()
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
            _rootContainer.RegisterInstance(_loadingView);

            var loadingScreen = _rootContainer.Resolve<UILoadingView>();
            _rootContainer.RegisterFactory(slc => new SceneLoaderService(loadingScreen)).AsSingle();

            _rootContainer.RegisterFactory(ssc => new SaveSystemContext(SaveSystemStrategyFactory.CreateStrategy())).AsSingle();

            var saveSystemContex = _rootContainer.Resolve<SaveSystemContext>();
            _rootContainer.RegisterFactory(gws => new GameWorldState(saveSystemContex)).AsSingle();
        }

        private async UniTask Run()
        {
            await _rootContainer.Resolve<GameWorldState>().StartAsyncTasks();

            _sceneNavigatorService.Start();
        }

        private static void HandleApplicationQuit()
        {
            _instance._rootContainer.Resolve<GameWorldState>().Dispose();
            _instance._sceneNavigatorService.Dispose();
            _instance._rootContainer.Dispose();
            YG2.GameplayStop();
        }
    }
}