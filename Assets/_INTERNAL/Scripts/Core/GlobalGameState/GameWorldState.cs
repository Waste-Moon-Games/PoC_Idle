using Core.SaveSystemBase;
using Cysharp.Threading.Tasks;

#if UNITY_WEBGL
using YG;
#endif

namespace Core.GlobalGameState
{
    public class GameWorldState
    {
        private readonly PlayerState _playerState;

        public PlayerState PlayerState => _playerState;

#if UNITY_WEBGL
        private bool _isRuLanguage = true;
        public bool IsRuLanguage => _isRuLanguage;
#endif

        public GameWorldState(SaveSystemContext saveSystemContext)
        {
            _playerState = new(saveSystemContext);
#if UNITY_WEBGL
            YG2.onCorrectLang += HandleCurrentLanguage;

            YG2.GetLanguage();
#endif
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
#if UNITY_WEBGL
            YG2.onCorrectLang -= HandleCurrentLanguage;
#endif
        }
#if UNITY_WEBGL
        private void HandleCurrentLanguage(string lang)
        {
            if (lang == "ru")
                _isRuLanguage = true;
            else if(lang == "en")
                _isRuLanguage = false;
        }
#endif
    }
}