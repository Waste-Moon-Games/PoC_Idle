using Cysharp.Threading.Tasks;
using R3;
using System.Threading;
using UI.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils.SceneLoader
{
    public class SceneLoaderService
    {
        private CancellationTokenSource _cts;

        private readonly UILoadingView _loadindScreen;

        private readonly Subject<float> _progressUpdated;
        private readonly Subject<string> _sceneLoaded;

        public Observable<float> OnProgressUpdated => _progressUpdated.AsObservable();
        public Observable<string> OnSceneLoaded => _sceneLoaded.AsObservable();

        public SceneLoaderService(UILoadingView loadindScreen)
        {
            _loadindScreen = loadindScreen;
            _progressUpdated = new Subject<float>();
            _sceneLoaded = new Subject<string>();
        }

        public void LoadScene(string sceneName)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            _cts = new();
            LoadSceneRoutine(sceneName, _cts.Token).Forget();
        }

        private async UniTask LoadSceneRoutine(string sceneName, CancellationToken token)
        {
            _loadindScreen.ShowLoadingScreen();

            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncOp.isDone && !token.IsCancellationRequested)
            {
                float progress = Mathf.Clamp01(asyncOp.progress / 0.9f);
                _loadindScreen.SetLoadingProgress(progress);
                _progressUpdated.OnNext(progress);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            token.ThrowIfCancellationRequested();
            _loadindScreen.HideLoadingScreen();

            _sceneLoaded.OnNext(sceneName);
        }
    }
}