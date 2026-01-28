namespace Core.GlobalGameState
{
    public class GameWorldState
    {
        private readonly PlayerState _playerState;

        public PlayerState PlayerState => _playerState;

        public GameWorldState()
        {
            _playerState = new();
        }

        public void StartAsyncTasks() => _playerState.StartAsyncTasks();

        public void Dispose()
        {
            _playerState.StopAsyncTasks();
            _playerState.Dispose();
        }
    }
}