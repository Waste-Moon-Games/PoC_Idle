namespace Core.LevelingSystem
{
    [System.Serializable]
    public class RewardByLevel : BaseReward
    {
        public int RewardRequiredLevel => base.RequiredLevel;
        public float RewardAmount => base.Amount;
        public RewardState RewardState => base.State;
        public RewardType RewardType => base.Type;
    }

    public class RewardByLevelRuntime : BaseReward
    {
        private int _rewardRequiredLevel;
        private float _rewardAmount;
        private RewardState _rewardState;
        private RewardType _rewardType;

        public string RewardID => base.ID;
        public int RewardRequiredLevel => _rewardRequiredLevel;
        public float RewardAmount => _rewardAmount;
        public RewardState RewardState => _rewardState;
        public RewardType RewardType => _rewardType;

        public RewardByLevelRuntime(RewardByLevel source)
        {
            _rewardRequiredLevel = source.RewardRequiredLevel;
            _rewardAmount = source.RewardAmount;
            _rewardState = source.RewardState;
            _rewardType = source.RewardType;
        }
    }
}