using Core.Shop.Base;
using System.Collections.Generic;
using UnityEngine;

namespace SO.ShopConfigs
{
    [CreateAssetMenu(fileName = "ShopItemsConfig", menuName = "Configs/Shop/ShopItemsConfig")]
    public class ShopItemsConfig : ScriptableObject
    {
        [SerializeField] private ShopRatesConfig _ratesConfig;

        [field: SerializeField] public List<ItemModelConfig> Items { get; private set; }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _ratesConfig.SyncWith(Items);
        }
#endif
    }
}