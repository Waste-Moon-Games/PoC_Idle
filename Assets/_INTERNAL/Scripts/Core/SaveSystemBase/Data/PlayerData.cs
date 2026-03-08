using System.Collections.Generic;

namespace Core.SaveSystemBase.Data
{
    [System.Serializable]
    public class PlayerData
    {
        public float Coins = 0f;
        public float Gems = 0f;

        public float PlayerClickAmount = 1f;
        public float PassiveIncomeAmount = 0f;

        public float TrippleClickChance = 0.01f;

        public int Level = 0;
        public int GainedExpPerClick = 1;
        public int CurrentExp = 0;
        public int ExpToLevelUp = 0;

        public Dictionary<string, Dictionary<string, ItemUpgradeData>> PurchasedUpgradesByShops = new();
        public List<RewardData> ReceivedRewards = new();
        public List<RewardData> CyclicRewards = new();
    }
}