using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SO.ShopConfigs
{
    [CreateAssetMenu(fileName = "ShopConfigsDatabase", menuName = "Configs/Shop/ShopConfigsDatabase")]
    public class ShopConfigsDatabase : ScriptableObject
    {
        [field: SerializeField] public List<ShopItemsConfig> ItemsConfigs { get; private set; }
        [field: SerializeField] public List<ShopRatesConfig> RatesConfigs { get; private set; }

        public ShopItemsConfig GetItemsConfigByID(string id)
        {
            ShopItemsConfig config = ItemsConfigs.FirstOrDefault(c => c.ShopID == id);

            return config;
        }

        public ShopRatesConfig GetRatesConfigByID(string id)
        {
            ShopRatesConfig config = RatesConfigs.FirstOrDefault(c => c.ShopID == id);

            return config;
        }
    }
}