using Core.GlobalGameState;
using R3;
using UnityEngine;
using UnityEngine.Audio;

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

            _audioSystemService.StartPlayMainThemeMusic();
        }

        private void HandleSoundToPlay(Sound sound) => _sfxSource.PlayOneShot(sound.Clip);

        private void HandleMainThemeToPlay(Sound sound)
        {
            _musicSource.clip = sound.Clip;
            _musicSource.Play();
        }
    }
}