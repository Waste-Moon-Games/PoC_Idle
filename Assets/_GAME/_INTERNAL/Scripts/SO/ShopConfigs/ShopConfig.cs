using Core.Shop.Base;
using System.Collections.Generic;
using UnityEngine;

namespace SO.ShopConfigs
{
    [CreateAssetMenu(fileName = "ShopItemsConfig", menuName = "Configs/Shop/ShopItemsConfig")]
    public class ShopConfig : ScriptableObject
    {
        [field: SerializeField] public string ShopID { get; private set; }
        [field: SerializeField] public float ShopOpenAnimationDuration { get; private set; } = 0.75f;
        [field: SerializeField] public bool OpenedByDefault { get; private set; }
        [field: SerializeField] public List<ItemModelConfig> Items { get; private set; } = new();
    }
}
