using UnityEngine;

namespace SO.AnimationConfigs
{
    [CreateAssetMenu(menuName = "Configs/Animations/Coin Flow Animation Config", fileName = "CoinFlowAnimationConfig")]
    public class CoinFlowAnimationConfig : ScriptableObject
    {
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float Amplitude { get; private set; }
    }
}