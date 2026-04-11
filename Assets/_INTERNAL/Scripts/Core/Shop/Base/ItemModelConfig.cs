using Core.Common.Player;
using UnityEngine;
using Utils.Localization;

namespace Core.Shop.Base
{
    [CreateAssetMenu(fileName = "ItemConfig", menuName = "Configs/Shop/ItemConfig")]
    public class ItemModelConfig : ScriptableObject
    {
        [field: Header("View Setup")]
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public LocalizedText Names { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public Sprite CommonCurrencyIcon { get; private set; }
        [field: SerializeField] public Sprite GemsCurrencyIcon { get; private set; }
        [field: SerializeField] public ItemType Type { get; private set; }
        [field: SerializeField] public CurrencyType CurrencyType { get; private set; } = CurrencyType.Coins;
        [field: SerializeField] public LocalizedText Descriptions { get; private set; }

        [field: Header("Initial Runtime Values")]
        [field: SerializeField] public float StartPrice { get; private set; } = 1f;
        [field: SerializeField] public float StartUpgradeAmount { get; private set; } = 1f;
        [field: SerializeField] public int StartLevel { get; private set; }
        [field: SerializeField] public bool IsOpenedByDefault { get; private set; }

        [field: Header("Upgrade Rates")]
        [field: SerializeField] public float PriceRate { get; private set; } = 1f;
        [field: SerializeField] public float BonusRate { get; private set; } = 1f;
    }
}
