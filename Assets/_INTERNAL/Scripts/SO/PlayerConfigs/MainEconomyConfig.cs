using UnityEngine;

namespace SO.PlayerConfigs
{
    [CreateAssetMenu(fileName = "MainEconomyConfig", menuName = "Configs/Economy/MainEconomyConfig")]
    public class MainEconomyConfig : ScriptableObject
    {
        #region Initial Variables
        [field: SerializeField, Range(0f, float.PositiveInfinity)] public float InitialCoinsWalletAmount { get; private set; }
        [field: SerializeField] public int InitialGemsWalletAmount { get; private set; }
        [field: SerializeField, Range(1f, float.PositiveInfinity)] public float InitialPlayerClickAmount { get; private set; }
        [field: SerializeField] public int InitialGemsClickRewardAmount { get; private set; }
        #endregion

        #region Delays
        [field: SerializeField] public float PassiveIncomeDelay { get; private set; }
        #endregion

        #region Chances
        [field: SerializeField, Range(0.01f, 0.85f)] public float InitialMinTrippleClickChance { get; private set; }
        [field: SerializeField, Range(0.01f, 0.9f)] public float InitialCurrentTrippleClickChance { get; private set; }
        [field: SerializeField, Range(0.01f, 0.9f)] public float MaxTrippleClickChance { get; private set; }
        [field: SerializeField, Range(0.01f, 1f)] public float InitialGemsClickRewardChance { get; private set; }
        [field: SerializeField, Range(0.01f, 1f)] public float MaxGemsClickRewardChance { get; private set; }
        #endregion
    }
}