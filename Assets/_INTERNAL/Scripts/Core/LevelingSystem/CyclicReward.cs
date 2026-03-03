namespace Core.LevelingSystem
{
    [System.Serializable]
    public class CyclicReward : BaseReward
    {
        public int RewardRequiredLevel => RequiredLevel;
        public float RewardAmount => Amount;
        public RewardType RewardType => Type;
    }

    public class CyclicRewardRuntime : BaseReward
    {
        public int RewardRequiredLevel => base.RequiredLevel;
        public float RewardAmount => base.Amount;
        public RewardType RewardType => base.Type;

        public CyclicRewardRuntime(CyclicReward source)
        {
            base.RequiredLevel = source.RewardRequiredLevel;
            base.Amount = source.RewardAmount;
            base.Type = source.RewardType;
            base.State = RewardState.Unlocked;
        }

        public CyclicRewardRuntime(float newAmount, int newLevel, RewardType type)
        {
            base.State = RewardState.Unlocked;

            base.Type = type;

            base.Amount = newAmount;
            base.RequiredLevel = newLevel;

            base.ID = $"bonus_{RequiredLevel}_{Type}";
        }
    }
}