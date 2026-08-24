using Cysharp.Threading.Tasks;
using R3;
using SO.GameConfigs;
using System;
using System.Threading;
using UI.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils.SceneLoader
{
    public class SceneLoaderService
    {
        private CancellationTokenSource _cts;

        private readonly float _minLoadingTime;
        private readonly UILoadingView _loadindScreen;

        private readonly Subject<float> _progressUpdated;
        private readonly Subject<string> _sceneLoaded;

        public Observable<float> OnProgressUpdated => _progressUpdated.AsObservable();
        public Observable<string> OnSceneLoaded => _sceneLoaded.AsObservable();

        public SceneLoaderService(UILoadingView loadindScreen, LoadingConfig config)
        {
            _loadindScreen = loadindScreen;
            _minLoadingTime = config.MinLoadingTime;

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
            float startTime = Time.time;

            _loadindScreen.ShowLoadingScreen();

            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
            asyncOp.allowSceneActivation = false;

            float currentProgress = 0f;

            while (!asyncOp.isDone && !token.IsCancellationRequested)
            {
                float rawProgress = Mathf.Clamp01(asyncOp.progress / 0.9f);

                while (currentProgress < rawProgress)
                {
                    currentProgress += Time.deltaTime * 1.5f;
                    currentProgress = Mathf.Min(currentProgress, rawProgress);
                    _loadindScreen.SetLoadingProgress(currentProgress);
                    await UniTask.NextFrame();
                }

                if(asyncOp.progress >= 0.9f)
                {
                    float elapsedTime = Time.time - startTime;
                    if(elapsedTime >= _minLoadingTime)
                    {
                        currentProgress = 1f;
                        _loadindScreen.SetLoadingProgress(currentProgress);
                        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

                        asyncOp.allowSceneActivation = true;
                    }
                }
                
                _progressUpdated.OnNext(rawProgress);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            token.ThrowIfCancellationRequested();
            _loadindScreen.HideLoadingScreen();

            _sceneLoaded.OnNext(sceneName);
        }
    }
}