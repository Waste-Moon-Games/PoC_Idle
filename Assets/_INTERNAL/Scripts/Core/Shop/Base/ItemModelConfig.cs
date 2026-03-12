using Core.Common.Player;
using UnityEngine;

namespace Core.Shop.Base
{
    [CreateAssetMenu(fileName = "ItemConfig", menuName = "Configs/Shop/ItemConfig")]
    public class ItemModelConfig : ScriptableObject
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public ItemType Type { get; private set; }
        [field: SerializeField] public CurrencyType CurrencyType { get; private set; } = CurrencyType.Coins;

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
