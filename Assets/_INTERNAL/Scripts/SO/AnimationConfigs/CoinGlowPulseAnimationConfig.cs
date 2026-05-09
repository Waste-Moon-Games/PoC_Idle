using UnityEngine;

namespace SO.AnimationConfigs
{
    [CreateAssetMenu(menuName = "Configs/Animations/Coin Glow Pulse Animation Config", fileName = "CoinGlowPulseAnimationConfig")]
    public class CoinGlowPulseAnimationConfig : ScriptableObject
    {
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float MinScale { get; private set; }
        [field: SerializeField] public float MaxScale { get; private set; }
    }
}