using Core.AudioSystemCommon;
using UnityEngine;

namespace SO.AudioSystemConfigs
{
    [CreateAssetMenu(menuName = "Configs/Audio System/Sound Config", fileName = "DefaultSoundConfig")]
    public class SoundConfig : ScriptableObject
    {
        [field: SerializeField] public SoundData SoundData { get; private set; }
    }
}