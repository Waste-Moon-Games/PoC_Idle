using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SO.ShopConfigs
{
    [CreateAssetMenu(fileName = "ShopConfigsDatabase", menuName = "Configs/Shop/ShopConfigsDatabase")]
    public class ShopConfigsDatabase : ScriptableObject
    {
        [field: SerializeField] public List<ShopConfig> ItemsConfigs { get; private set; }

        public ShopConfig GetItemsConfigByID(string id)
        {
            ShopConfig config = ItemsConfigs.FirstOrDefault(c => c.ShopID == id);

            return config;
        }
    }
}