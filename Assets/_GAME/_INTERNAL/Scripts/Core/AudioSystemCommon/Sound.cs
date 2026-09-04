using UnityEngine;
using UnityEngine.Audio;

namespace Core.AudioSystemCommon
{
    public class Sound
    {
        private readonly string _id;
        private readonly AudioClip _clip;
        private readonly SoundType _type;
        private readonly AudioMixerGroup _mixerGroup;

        private readonly float _volume;
        private readonly float _pitch;
        private bool _isPlaying;

        public string ID => _id;
        public AudioClip Clip => _clip;
        public SoundType Type => _type;
        public AudioMixerGroup MixerGroup => _mixerGroup;
        public float Volume => _volume;
        public float Pitch => _pitch;
        public bool IsPlaying => _isPlaying;

        public Sound(SoundData sourceData)
        {
            _id = sourceData.ClipID;
            _clip = sourceData.Clip;
            _type = sourceData.Type;
            _mixerGroup = sourceData.MixerGroup;

            _volume = sourceData.InitVolume;
            _pitch = sourceData.InitPitch;
        }

        public void MarkPlayingFlag(bool value) => _isPlaying = value;
    }
}