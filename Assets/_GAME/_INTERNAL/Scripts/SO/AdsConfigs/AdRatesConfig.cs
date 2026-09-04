using UnityEngine;

namespace SO.AdsConfigs
{
    [CreateAssetMenu(menuName = "Configs/Ads/Ad Rates Config", fileName = "AdRatesConfig")]
    public class AdRatesConfig : ScriptableObject
    {
        [field: SerializeField, Range(0f, 1f)] public float InterstitialAdShowChance { get; private set; } = 0.5f;
    }
}