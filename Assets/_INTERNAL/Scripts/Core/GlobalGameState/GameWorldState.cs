using Utils.ModCoroutines;

namespace Core.GlobalGameState
{
    public class GameWorldState
    {
        private readonly PlayerState _playerState;

        public PlayerState PlayerState => _playerState;

        public GameWorldState(Coroutines coroutines)
        {
            _playerState = new(coroutines);
        }
    }
}