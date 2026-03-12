using System.Collections.Generic;

namespace Core.SaveSystemBase.Data
{
    [System.Serializable]
    public class ShopStateData
    {
        public string ShopID;
        public bool IsOpened;
        public List<ItemUpgradeData> Items = new();
    }
}
