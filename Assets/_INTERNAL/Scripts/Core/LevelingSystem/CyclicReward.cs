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
        private int _rewardRequiredLevel;
        private float _rewardAmount;
        private RewardType _rewardType;

        public int RewardRequiredLevel => _rewardRequiredLevel;
        public float RewardAmount => _rewardAmount;
        public RewardType RewardType => _rewardType;

        public CyclicRewardRuntime(CyclicReward source)
        {
            _rewardRequiredLevel = source.RewardRequiredLevel;
            _rewardAmount = source.RewardAmount;
            _rewardType = source.RewardType;
        }

        public CyclicRewardRuntime(float newAmount, int newLevel, RewardType type)
        {
            State = RewardState.Unlocked;

            Type = type;

            Amount = newAmount;
            RequiredLevel = newLevel;

            ID = $"bonus_{RequiredLevel}_{Type}";
        }
    }
}