using Core.GlobalGameState.Services;
using Core.SaveSystemBase;
using Cysharp.Threading.Tasks;

namespace Core.GlobalGameState
{
    public class GameWorldState
    {
        private readonly PlayerState _playerState;
        private readonly LocalizationService _localizationService;

        public PlayerState PlayerState => _playerState;
        public LocalizationService LocalizationService => _localizationService;

        public GameWorldState(SaveSystemContext saveSystemContext)
        {
            _playerState = new(saveSystemContext);
            _localizationService = new LocalizationService();
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
            _localizationService.Dispose();
        }
    }
}