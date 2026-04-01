using Core.Enums;

namespace Core.Common.Data
{
    [System.Serializable]
    public class BonusItemData
    {
        public BonusItemType Type;
        public string RuDescription;
        public string EnDescription;
        public float Amount;
        public float BonusDuration;
    }
}