using System.Collections.Generic;
using UnityEngine;

namespace SO.AudioSystemConfigs
{
    [CreateAssetMenu(menuName = "Configs/Audio System/Sounds Collection Config", fileName = "SoundsCollectionConfig")]
    public class SoundsCollectionConfig : ScriptableObject
    {
        [field: SerializeField] public List<SoundConfig> Sounds { get; private set; } = new();
        [field: SerializeField] public SoundConfig MainTheme { get; private set; }
    }
}