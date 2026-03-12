using System.Collections.Generic;

namespace Core.SaveSystemBase.Data
{
    [System.Serializable]
    public class ShopStateData
    {
        public string ShopID;
        public Dictionary<int, ItemUpgradeData> Items = new();
    }
}