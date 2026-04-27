using Core.AdsSystem;
using Core.GlobalGameState;
using Core.MonoContainers;
using Core.SaveSystemBase;

using Cysharp.Threading.Tasks;
using SO.AdsConfigs;
using SO.AudioSystemConfigs;
using System;
using UI.Common;

using UnityEngine;
using Object = UnityEngine.Object;

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
        public static void AutoStart()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            _instance = new GameEntry();

            RunAsync().Forget();

#if UNITY_ANDROID
            Application.quitting += HandleApplicationQuit;
#endif
        }

        private static async UniTaskVoid RunAsync()
        {
            try
            {
                await _instance.Run();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"GameEntry failed: {ex.GetType().Name}: {ex.Message}");
            }
        }

        private GameEntry()
        {
            var loadingScreenPrefab = Resources.Load<UILoadingView>("UI/Common/UILoadingView");
            if (loadingScreenPrefab == null)
                throw new InvalidOperationException("Resources/UI/Common/UILoadingView prefab not found");

            _loadingView = Object.Instantiate(loadingScreenPrefab);

            RegisterGlobalServices();

            var scl = _rootContainer.Resolve<SceneLoaderService>();
            _sceneNavigatorService = new(scl, _rootContainer);
        }

        private void RegisterGlobalServices()
        {
            var adRatesConfig = Resources.Load<AdRatesConfig>("Configs/Ads/AdRatesConfig");
            var soundsLibrary = Resources.Load<SoundsCollectionConfig>("Configs/AudioSystem/SoundsCollectionConfig");

            if (adRatesConfig == null)
                throw new InvalidOperationException("Resources/Configs/Ads/AdRatesConfig asset not found");
            if (soundsLibrary == null)
                throw new InvalidOperationException("Resources/Configs/AudioSystem/SoundsCollectionConfig asset not found");

            _rootContainer.RegisterInstance(_loadingView);

            var loadingScreen = _rootContainer.Resolve<UILoadingView>();
            _rootContainer.RegisterFactory(slc => new SceneLoaderService(loadingScreen)).AsSingle();

            _rootContainer.RegisterFactory(ssc => new SaveSystemContext(SaveSystemStrategyFactory.CreateStrategy())).AsSingle();
            _rootContainer.RegisterFactory(ads => new AdsSystemContext(AdsStrategyFactory.CreateStrategy(), adRatesConfig.InterstitialAdShowChance)).AsSingle();
            _rootContainer.RegisterFactory(ass => new AudioSystemService(soundsLibrary.Sounds, soundsLibrary.MainTheme)).AsSingle();

            var saveSystemContext = _rootContainer.Resolve<SaveSystemContext>();
            var adsSystemContext = _rootContainer.Resolve<AdsSystemContext>();
            var audioSystemService = _rootContainer.Resolve<AudioSystemService>();
            _rootContainer.RegisterFactory(gws => new GameWorldState(saveSystemContext, adsSystemContext, audioSystemService)).AsSingle();
        }

        private async UniTask Run()
        {
            var gameWorldState = _rootContainer.Resolve<GameWorldState>();
            await gameWorldState.InitPlayerState();
            gameWorldState.StartAsyncOperations();

            _sceneNavigatorService.Start();
            
            var uiSfxSystemsPrefab = Resources.Load<UISFXMonoContainer>("Other/Utils/UISFXSystems");
            if (uiSfxSystemsPrefab == null)
                throw new InvalidOperationException("Resources/Other/Utils/UISFXSystems prefab not found");

            var uiSfxSystem = Object.Instantiate(uiSfxSystemsPrefab);

#if UNITY_WEBGL
            var monoDisposeContainerPrefab = Resources.Load<DisposeContainer>("Other/Utils/[DISPOSE_CONTAINER]");
            if (monoDisposeContainerPrefab == null)
                throw new InvalidOperationException("Resources/Other/Utils/[DISPOSE_CONTAINER] prefab not found");

            var monoDisposeContainer = Object.Instantiate(monoDisposeContainerPrefab);
            var infoYG = YG2.infoYG;
            if (infoYG != null && infoYG.QuitGameEvent != null)
            {
                infoYG.QuitGameEvent.objectName = monoDisposeContainer.name;
                infoYG.QuitGameEvent.methodName = nameof(monoDisposeContainer.Dispose);
            }
            else
            {
                Debug.LogWarning("YG2.infoYG or QuitGameEvent settings are null. Skip runtime quit binding.");
            }
            monoDisposeContainer.BindDisposableComponents(_rootContainer, _sceneNavigatorService, gameWorldState);
#endif

            var audioPlayer = uiSfxSystem.AudioPlayer;
            gameWorldState.AudioSystemService.Initialization();
            audioPlayer.BindAudioSystemService(gameWorldState.AudioSystemService);
        }

#if UNITY_ANDROID
        private static void HandleApplicationQuit()
        {
            _instance._rootContainer.Resolve<GameWorldState>().Dispose();
            _instance._sceneNavigatorService.Dispose();
            _instance._rootContainer.Dispose();
        }
#endif
    }
}