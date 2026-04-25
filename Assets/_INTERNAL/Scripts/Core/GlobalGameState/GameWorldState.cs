using Core.AdsSystem;
using Core.GlobalGameState.Services;
using Core.SaveSystemBase;
using Cysharp.Threading.Tasks;

namespace Core.GlobalGameState
{
    public class GameWorldState
    {
        private readonly PlayerState _playerState;
        private readonly LocalizationService _localizationService;
        private readonly AudioSystemService _audioSystemService;

        public PlayerState PlayerState => _playerState;
        public LocalizationService LocalizationService => _localizationService;
        public AudioSystemService AudioSystemService => _audioSystemService;

        public GameWorldState(SaveSystemContext saveSystemContext, AdsSystemContext adsSystemContext, AudioSystemService audioSystemService)
        {
            _localizationService = new LocalizationService();
            _playerState = new(saveSystemContext, adsSystemContext, _localizationService.CurrentLanguage, audioSystemService);
            _audioSystemService = audioSystemService;
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
            _audioSystemService.Dispose();
        }
    }
}