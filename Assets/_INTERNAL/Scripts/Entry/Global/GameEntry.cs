using Core.AdsSystem;
using Core.GlobalGameState;
using Core.MonoContainers;
using Core.SaveSystemBase;

using Cysharp.Threading.Tasks;
using SO.AdsConfigs;
using SO.AudioSystemConfigs;
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
            var soundsLibrary = Resources.Load<SoundsCollectionConfig>("Configs/AudioSystem/SoundsCollectionConfig");
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

            var uiSfxSystemsPrefab = Resources.Load<UISFXMonoContainer>("Other/Utils/UI_SFX Systems");
            var uiSfxSystem = Object.Instantiate(uiSfxSystemsPrefab);

            var audioPlayer = uiSfxSystem.AudioPlayer;
            audioPlayer.BindAudioSystemService(gameWorldState.AudioSystemService);
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