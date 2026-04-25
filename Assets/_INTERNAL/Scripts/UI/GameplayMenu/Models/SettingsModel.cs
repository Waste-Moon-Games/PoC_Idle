using Common.MVVM;
using Core.GlobalGameState;
using R3;
using UnityEngine;

namespace UI.GameplayMenu.Models
{
    public class SettingsModel : IModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly BehaviorSubject<float> _sfxVolumeChangedSignal;
        private readonly BehaviorSubject<float> _musicVolumeChangedSignal;

        private readonly BehaviorSubject<bool> _sfxStateChangedSignal;
        private readonly BehaviorSubject<bool> _musicStateChangedSignal;
        private readonly BehaviorSubject<bool> _settingsWindowStateChangedSignal;

        private readonly AudioSystemService _audioSystemService;

        private float _currentSfxVolume;
        private float _currentMusicVolume;

        private bool _sfxState = true;
        private bool _musicState = true;

        private bool _settingsWindowState = false;

        public Observable<float> SFXVolumeChangedSignal => _sfxVolumeChangedSignal.AsObservable();
        public Observable<float> MusicVolumeChangedSignal => _musicVolumeChangedSignal.AsObservable();

        public Observable<bool> SFXStateChangedSignal => _sfxStateChangedSignal.AsObservable();
        public Observable<bool> MusicStateChangedSignal => _musicStateChangedSignal.AsObservable();
        public Observable<bool> SettingsWindowStateChangedSignal => _settingsWindowStateChangedSignal.AsObservable();

        public SettingsModel(AudioSystemService audioSystemService)
        {
            _audioSystemService = audioSystemService;

            _audioSystemService.SFXVolumeChangedSignal.Subscribe(HandleChangedSFXVolume).AddTo(_disposables);
            _audioSystemService.MusicVolumeChangedSignal.Subscribe(HandleChangedMusicVolume).AddTo(_disposables);

            _audioSystemService.SFXStateChangedSignal.Subscribe(HandleSFXChangedState).AddTo(_disposables);
            _audioSystemService.MusicStateChangedSignal.Subscribe(HandleMusicChagedState).AddTo(_disposables);

            _audioSystemService.SFXVolumeChange(_currentSfxVolume);
            _audioSystemService.MusicVolumeChange(_currentMusicVolume);

            _settingsWindowStateChangedSignal = new(_settingsWindowState);

            _sfxVolumeChangedSignal = new(_currentSfxVolume);
            _musicVolumeChangedSignal = new(_currentMusicVolume);

            _sfxStateChangedSignal = new(_sfxState);
            _musicStateChangedSignal = new(_musicState);
        }

        public void BindNavigationActions(Observable<MainMenuEvents> settingsButtonClickSignal)
        {
            settingsButtonClickSignal.Subscribe(_ =>
            {
                HandleSettingsButtonClick();
            }).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void SetSFXVolume(float volume) => _audioSystemService.SFXVolumeChange(volume);

        public void SetMusicVolume(float volume) => _audioSystemService.MusicVolumeChange(volume);

        public void ToggleSFXState(bool state) => _audioSystemService.SFXStateChange(state);

        public void ToggleMusicState(bool state) => _audioSystemService.MusicStateChange(state);

        public void Close()
        {
            _settingsWindowState = false;
            _settingsWindowStateChangedSignal.OnNext(_settingsWindowState);
        }

        public void OpenVK()
        {
#if UNITY_ANDROID
            Application.OpenURL("https://vk.com/wastemoon_games");
#endif
        }

        private void HandleSettingsButtonClick()
        {
            _settingsWindowState = true;
            _settingsWindowStateChangedSignal.OnNext(_settingsWindowState);
        }

        private void HandleChangedSFXVolume(float volume)
        {
            _currentSfxVolume = volume;
            _sfxVolumeChangedSignal.OnNext(_currentSfxVolume);
        }

        private void HandleChangedMusicVolume(float volume)
        {
            _currentMusicVolume = volume;
            _musicVolumeChangedSignal.OnNext(_currentMusicVolume);
        }

        private void HandleSFXChangedState(bool state)
        {
            _sfxState = state;
            _sfxStateChangedSignal.OnNext(_sfxState);
        }

        private void HandleMusicChagedState(bool state)
        {
            _musicState = state;
            _musicStateChangedSignal.OnNext(_musicState);
        }
    }
}