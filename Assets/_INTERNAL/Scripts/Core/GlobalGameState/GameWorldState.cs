using Core.SaveSystemBase;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.GlobalGameState
{
    public class GameWorldState
    {
        private readonly PlayerState _playerState;

        public PlayerState PlayerState => _playerState;

        public GameWorldState(SaveSystemContext saveSystemContext)
        {
            _playerState = new(saveSystemContext);
        }

        public async UniTask InitPlayerState()
        {
            await _playerState.InitializeAsync();
        }

        public void StartAsyncOperations()
        {
            _playerState.StartAsyncOperations();
        }

        public void Dispose()
        {
            _playerState.StopAsyncTasks();
            _playerState.Dispose();
        }
    }
}