namespace Core.LevelingSystem
{
    [System.Serializable]
    public class CyclicReward : BaseReward
    {
        public int RewardRequiredLevel => RequiredLevel;
        public float RewardAmount => Amount;
        public RewardType RewardType => Type;

        public CyclicReward(float newAmount, int newLevel, RewardType type)
        {
            State = RewardState.Unlocked;

            Type = type;

            Amount = newAmount;
            RequiredLevel = newLevel;

            ID = $"bonus_{RequiredLevel}_{Type}";
        }
    }
}