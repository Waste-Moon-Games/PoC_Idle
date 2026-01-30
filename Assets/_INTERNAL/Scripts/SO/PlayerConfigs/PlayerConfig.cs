using UnityEngine;

namespace SO.PlayerConfigs
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/Player/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
	{
        [field: SerializeField] public int InitPlayerLevel { get; private set; }
        [field: SerializeField] public int InitGainedPlayerExpPerClick { get; private set; }
        [field: SerializeField] public int InitExpToLevelUp { get; private set; }
        [field: SerializeField] public float ExpIncreaseMultiplier { get; private set; }

        [field: SerializeField] public float PlayerBonusGauge { get; private set; }
        [field: SerializeField] public float InitPlayerBonusPerClick { get; private set; }
        [field: SerializeField] public float BonusClickMultiplier { get; private set; }
        [field: SerializeField] public float DecreaseBonusGaugeDelay { get; private set; }
        [field: SerializeField] public float DecreaseActiveBonusGaugeDelay { get; private set; }
	}
}