using Core.GlobalGameState;
using R3;
using UnityEngine;
using UnityEngine.Audio;
using Utils.Audio;

namespace Core.AudioSystemCommon.MonoObjects
{
    [RequireComponent(typeof(AudioListener))]
    public class AudioPlayer : MonoBehaviour
    {
        private readonly CompositeDisposable _disposables = new();

        [Header("Mixers Setup")]
        [SerializeField] private string _sfxMixerName = "SFX";
        [SerializeField] private string _musicMixerName = "Music";
        [SerializeField] private AudioMixer _mainMixer;

        [Space(5), Header("Sources")]
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _musicSource;

        private AudioSystemService _audioSystemService;

        private void Awake()
        {
            if (_sfxSource.outputAudioMixerGroup == null)
                throw new System.ArgumentNullException(nameof(_sfxSource.outputAudioMixerGroup), "Output mixer group is null (SFX)");

            if (_musicSource.outputAudioMixerGroup == null)
                throw new System.ArgumentNullException(nameof(_musicSource.outputAudioMixerGroup), "Output mixer group is null (Music)");
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        public void BindAudioSystemService(AudioSystemService audioSystemService)
        {
            _audioSystemService = audioSystemService;

            _audioSystemService.SoundPlaySignal
                .Subscribe(sound =>
                {
                    if (sound.Type != SoundType.Main_Music)
                        HandleSoundToPlay(sound);
                    else
                        HandleMainThemeToPlay(sound);
                })
                .AddTo(_disposables);

            _audioSystemService.SFXVolumeChangedSignal.Subscribe(HandleChangedSFXVolume).AddTo(_disposables);
            _audioSystemService.MusicVolumeChangedSignal.Subscribe(HandleChangedMusicVolume).AddTo(_disposables);

            _audioSystemService.SFXStateChangedSignal.Subscribe(HandleChangedSFXState).AddTo(_disposables);
            _audioSystemService.MusicStateChangedSignal.Subscribe(HandleChangedMusicState).AddTo(_disposables);
        }

        private void HandleChangedSFXState(bool state) => _sfxSource.enabled = state;

        private void HandleChangedMusicState(bool state) => _musicSource.enabled = state;

        private void HandleChangedSFXVolume(float volume)
        {
            var sfxVolume = Mathf.Clamp01(volume);
            _mainMixer.SetFloat(_sfxMixerName, AudioUtils.LinearToDB(sfxVolume));
        }

        private void HandleChangedMusicVolume(float volume)
        {
            var musicVolume = Mathf.Clamp01(volume);
            _mainMixer.SetFloat(_musicMixerName, AudioUtils.LinearToDB(musicVolume));
        }

        private void HandleSoundToPlay(Sound sound) => _sfxSource.PlayOneShot(sound.Clip);

        private void HandleMainThemeToPlay(Sound sound)
        {
            _musicSource.clip = sound.Clip;
            _musicSource.Play();
        }
    }
}