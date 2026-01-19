using Core.Consts;
using Core.Consts.Enums;
using Entry.Local.Gameplay;
using Entry.Local.Shop;
using R3;
using System;
using UI.GameplayMenu.Models;
using Utils.DI;
using Utils.SceneLoader;
using Object = UnityEngine.Object;

namespace Entry.Global
{
    public class SceneNavigatorService : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly SceneLoaderService _loaderService;
        private readonly DIContainer _rootContainer;

        private DIContainer _cachedContainer;

        public SceneNavigatorService(SceneLoaderService sceneLoaderService, DIContainer rootContainer)
        {
            _loaderService = sceneLoaderService;
            _rootContainer = rootContainer;
        }

        public void Start() => LoadScene(SceneNames.GAME);

        public void Dispose() => _disposables.Clear();

        private void LoadScene(string sceneName)
        {
            _cachedContainer?.Dispose();
            _cachedContainer = null;

            _loaderService.OnSceneLoaded
                .Take(1)
                .Subscribe(_ => OnSceneLoaded(sceneName))
                .AddTo(_disposables);

            _loaderService.LoadScene(sceneName);
        }

        private void OnSceneLoaded(string sceneName)
        {
            switch (sceneName)
            {
                case SceneNames.GAME:
                    CreateGameScene();
                    break;
                case SceneNames.SHOP:
                    CreateShopScene();
                    break;
            }
        }

        private void CreateGameScene()
        {
            var container = _cachedContainer = new(_rootContainer);
            var entryPoint = Object.FindFirstObjectByType<GameplayEntryPoint>();

            entryPoint.Run(container).Subscribe(action =>
            {
                switch (action)
                {
                    case MainMenuEvents.ShopClicked:
                        LoadScene(SceneNames.SHOP);
                        break;
                    //to do больше сцен/действий
                }
            }).AddTo(_disposables);
        }

        private void CreateShopScene()
        {
            var container = _cachedContainer = new(_rootContainer);
            var entryPoint = Object.FindFirstObjectByType<ShopEntryPoint>();

            entryPoint.Run(container)
                .Where(action => action == ShopEvents.Exit)
                .Subscribe(_ => LoadScene(SceneNames.GAME))
                .AddTo(_disposables);
        }
    }
}