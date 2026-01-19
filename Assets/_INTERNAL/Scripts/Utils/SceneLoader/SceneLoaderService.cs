using R3;
using System.Collections;
using UI.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.ModCoroutines;

namespace Utils.SceneLoader
{
    public class SceneLoaderService
    {
        private readonly UILoadingView _loadindScreen;
        private readonly Coroutines _coroutine;

        private readonly Subject<float> _progressUpdated;
        private readonly Subject<string> _sceneLoaded;

        public Observable<float> OnProgressUpdated => _progressUpdated.AsObservable();

        public Observable<string> OnSceneLoaded => _sceneLoaded.AsObservable();

        public SceneLoaderService(UILoadingView loadindScreen, Coroutines coroutine)
        {
            _loadindScreen = loadindScreen;
            _coroutine = coroutine;
            _progressUpdated = new Subject<float>();
            _sceneLoaded = new Subject<string>();
        }

        public void LoadScene(string sceneName)
        {
            _coroutine.StartRoutine(LoadSceneRoutine(sceneName));
        }

        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            _loadindScreen.ShowLoadingScreen();

            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
            asyncOp.allowSceneActivation = true;

            while (!asyncOp.isDone)
            {
                _loadindScreen.SetLoadingProgress(asyncOp.progress / 0.9f);
                _progressUpdated.OnNext(asyncOp.progress / 0.9f);
                yield return null;
            }

            _loadindScreen.HideLoadingScreen();

            _sceneLoaded.OnNext(sceneName);
        }
    }
}