using System.Collections.Generic;
using UnityEngine;

namespace SO.ShopConfigs
{
    [CreateAssetMenu(fileName = "ItemsTypeMultiplierConfig", menuName = "Configs/Shop/ItemsTypeMultiplierConfig")]
    public class ItemsTypeMultiplierConfig : ScriptableObject
    {
        [field: SerializeField] public List<float> UpgradesMultipliers { get; private set; }
        [field: SerializeField] public List<float> UpgradesPriceMultipliers { get; private set; }
    }
}