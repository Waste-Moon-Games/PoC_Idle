using UnityEngine;

namespace SO.EconomyConfigs
{
    [CreateAssetMenu(fileName = "MainEconomyConfig", menuName = "Configs/Economy/MainEconomyConfig")]
    public class MainEconomyConfig : ScriptableObject
    {
        [field: SerializeField, Range(0f, float.PositiveInfinity)] public float InitialPlayerWallet { get; private set; }
        [field: SerializeField, Range(1f, float.PositiveInfinity)] public float InitialPlayerClickAmount { get; private set; }

        [field: SerializeField, Range(0.01f, 0.85f)] public float InitialMinTrippleClickChance { get; private set; }
        [field: SerializeField, Range(0.01f, 0.9f)] public float InitialCurrentTrippleClickChance { get; private set; }
        [field: SerializeField, Range(0.01f, 0.9f)] public float MaxTrippleClickChance { get; private set; }
    }
}