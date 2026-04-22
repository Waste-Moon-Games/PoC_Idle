using UnityEngine;
using UnityEngine.Audio;

namespace Core.AudioSystemCommon
{
    [System.Serializable]
    public class SoundData
    {
        public string ClipID = "Default_Clip";
        public AudioMixerGroup MixerGroup;
        public SoundType Type = SoundType.Click;
        public AudioClip Clip;
        [Range(0f, 1f)] public float InitVolume  = 1.0f;
        public float InitPitch = 1.0f;
    }
}