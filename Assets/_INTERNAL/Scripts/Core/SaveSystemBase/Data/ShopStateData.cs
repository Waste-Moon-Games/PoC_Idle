using System.Collections.Generic;

namespace Core.SaveSystemBase.Data
{
    [System.Serializable]
    public class ShopStateData
    {
        public string ShopID;
        public List<ItemUpgradeData> Items = new();
    }
}