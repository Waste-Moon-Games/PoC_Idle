using Core.AudioSystemCommon;
using R3;
using SO.AudioSystemConfigs;
using System.Collections.Generic;

namespace Core.GlobalGameState
{
    public class AudioSystemService
    {
        private readonly Dictionary<string, Sound> _soundsLibrary = new();
        private readonly Sound _mainThemeMusic;

        private readonly Subject<Sound> _soundPlaySignal = new();

        public Observable<Sound> SoundPlaySignal => _soundPlaySignal.AsObservable();

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
    }
}