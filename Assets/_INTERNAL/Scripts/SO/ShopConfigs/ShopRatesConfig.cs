using Core.Shop.Base;
using System.Collections.Generic;
using UnityEngine;

namespace SO.ShopConfigs
{
    [CreateAssetMenu(fileName = "ShopRatesConfig", menuName = "Configs/Shop/ShopRatesConfig")]
    public class ShopRatesConfig : ScriptableObject
    {
        [field: SerializeField] public List<ItemRateEntry> Rates { get; private set; } = new();

        public void SyncWith(List<ItemModelConfig> itemsConfig)
        {
            foreach (var item in itemsConfig)
            {
                if (Rates.Exists(r => r.ItemID == item.Name))
                    continue;

                Rates.Add(new()
                {
                    ItemID = item.Name,
                    PriceRate = 1f,
                    BonusRate = 1f
                });
            }

            Rates.RemoveAll(r => !itemsConfig.Exists(i => i.Name == r.ItemID));

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}