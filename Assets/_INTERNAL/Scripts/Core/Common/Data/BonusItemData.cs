using Core.Enums;
using Utils.Localization;

namespace Core.Common.Data
{
    [System.Serializable]
    public class BonusItemData
    {
        public BonusItemType Type;
        public LocalizedText Descriptions;
        public float Amount;
        public float BonusDuration;
    }
}