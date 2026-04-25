#if UNITY_WEBGL
using Core.GlobalGameState;
using Entry.Global;
using UnityEngine;
using Utils.DI;
using YG;

namespace Core.MonoContainers
{
    public class DisposeContainer : MonoBehaviour
    {
        private DIContainer _rootContainer;
        private SceneNavigatorService _sceneNavigatorService;
        private GameWorldState _gameWorldState;

        public void BindDisposableComponents(DIContainer rootContainer, SceneNavigatorService sceneNavigatorService, GameWorldState gameWorldState)
        {
            _rootContainer = rootContainer;
            _sceneNavigatorService = sceneNavigatorService;
            _gameWorldState = gameWorldState;
        }

        public void Dispose()
        {
            YG2.GameplayStop();
            _gameWorldState?.Dispose();
            _sceneNavigatorService?.Dispose();
            _rootContainer?.Dispose();

            Debug.Log("[DISPOSE_CONTAINER] Disposed");
        }
    }
}
#endif