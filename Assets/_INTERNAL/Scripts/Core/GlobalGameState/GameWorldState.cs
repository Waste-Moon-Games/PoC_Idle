using Core.SaveSystemBase;
using Cysharp.Threading.Tasks;

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

        public async UniTask StartAsyncTasks()
        {
            await _playerState.StartAsyncTasks();
        }

        public void Dispose()
        {
            _playerState.StopAsyncTasks();
            _playerState.Dispose();
        }
    }
}