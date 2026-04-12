using Core.Enums;
using Utils.Localization;

namespace Core.Common.Data
{
    [System.Serializable]
    public class BonusItemData
    {
        public string ItemID;
        public BonusItemType Type;
        public LocalizedText Descriptions;
        public float Amount;
        public float BonusDuration;
    }
}