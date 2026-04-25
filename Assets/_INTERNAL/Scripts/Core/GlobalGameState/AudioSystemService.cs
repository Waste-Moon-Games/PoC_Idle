using Core.Consts;
using Core.AudioSystemCommon;
using R3;
using SO.AudioSystemConfigs;
using System.Collections.Generic;
using PlayerPrefs = RedefineYG.PlayerPrefs;

namespace Core.GlobalGameState
{
    public class AudioSystemService
    {
        private readonly Dictionary<string, Sound> _soundsLibrary = new();
        private readonly Sound _mainThemeMusic;

        private readonly Subject<Sound> _soundPlaySignal = new();

        private BehaviorSubject<float> _sfxVolumeChangedSignal;
        private BehaviorSubject<float> _musicVolumeChangedSignal;

        private BehaviorSubject<bool> _sfxStateChangedSignal;
        private BehaviorSubject<bool> _musicStateChangedSignal;

        private float _currentSFXVolume;
        private float _currentMusicVolume;

        private bool _currentSFXState;
        private bool _currentMusicState;

        public Observable<Sound> SoundPlaySignal => _soundPlaySignal.AsObservable();
        public Observable<float> SFXVolumeChangedSignal => _sfxVolumeChangedSignal.AsObservable();
        public Observable<float> MusicVolumeChangedSignal => _musicVolumeChangedSignal.AsObservable();
        public Observable<bool> SFXStateChangedSignal => _sfxStateChangedSignal.AsObservable();
        public Observable<bool> MusicStateChangedSignal => _musicStateChangedSignal.AsObservable();

        public AudioSystemService(List<SoundConfig> sources, SoundConfig mainThemeMusic)
        {
            if (sources.Count == 0)
                throw new System.ArgumentException("Sound sources is empty", nameof(sources));

            for (int i = 0; i < sources.Count; i++)
            {
                var soundSource = sources[i];
                var sound = new Sound(soundSource.SoundData);

                SoundsIds.SetNewIdByType(sound.ID, sound.Type);

                _soundsLibrary[sound.ID] = sound;
            }

            _mainThemeMusic = new(mainThemeMusic.SoundData);
        }

        public void Initialization()
        {
            LoadPrefs();

            _sfxVolumeChangedSignal = new(_currentSFXVolume);
            _musicVolumeChangedSignal = new(_currentMusicVolume);

            _sfxStateChangedSignal = new(_currentSFXState);
            _musicStateChangedSignal = new(_currentMusicState);
        }

        public void Dispose()
        {
            SavePrefs();
        }

        public void StartPlayMainThemeMusic()
        {
            if (_mainThemeMusic.IsPlaying)
                return;

            _mainThemeMusic.MarkPlayingFlag(true);
            _soundPlaySignal.OnNext(_mainThemeMusic);
        }

        public void PlaySoundByID(string soundId)
        {
            if(_soundsLibrary.TryGetValue(soundId, out Sound sound))
                _soundPlaySignal.OnNext(sound);
        }

        public void SFXVolumeChange(float volume)
        {
            _currentSFXVolume = volume;
            _sfxVolumeChangedSignal.OnNext(volume);
        }

        public void MusicVolumeChange(float volume)
        {
            _currentMusicVolume = volume;
            _musicVolumeChangedSignal.OnNext(volume);
        }

        public void SFXStateChange(bool state)
        {
            _currentSFXState = state;
            _sfxStateChangedSignal.OnNext(state);
        }

        public void MusicStateChange(bool state)
        {
            _currentMusicState = state;
            _musicStateChangedSignal.OnNext(state);
        }

        private void SavePrefs()
        {
            PlayerPrefs.SetInt(SettingsPlayerPrefsKeys.SFX_STATE, _currentSFXState ? 1 : 0);
            PlayerPrefs.SetInt(SettingsPlayerPrefsKeys.MUSIC_STATE, _currentMusicState ? 1 : 0);

            PlayerPrefs.SetFloat(SettingsPlayerPrefsKeys.SFX_VOLUME, _currentSFXVolume);
            PlayerPrefs.SetFloat(SettingsPlayerPrefsKeys.MUSIC_VOLUME, _currentMusicVolume);

            PlayerPrefs.Save();
        }

        private void LoadPrefs()
        {
            _currentSFXState = PlayerPrefs.GetInt(SettingsPlayerPrefsKeys.SFX_STATE, 1) == 1;
            _currentMusicState = PlayerPrefs.GetInt(SettingsPlayerPrefsKeys.MUSIC_STATE, 1) == 1;

            _currentSFXVolume = PlayerPrefs.GetFloat(SettingsPlayerPrefsKeys.SFX_VOLUME, 1f);
            _currentMusicVolume = PlayerPrefs.GetFloat(SettingsPlayerPrefsKeys.MUSIC_VOLUME, 1f);
        }
    }
}