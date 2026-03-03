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
        public string RewardID => base.ID;
        public int RewardRequiredLevel => base.RequiredLevel;
        public float RewardAmount => base.Amount;
        public RewardState RewardState => base.State;
        public RewardType RewardType => base.Type;

        public RewardByLevelRuntime(RewardByLevel source)
        {
            base.ID = source.RewardRequiredLevel.ToString();
            base.RequiredLevel = source.RewardRequiredLevel;
            base.Amount = source.RewardAmount;
            base.Type = source.RewardType;
            base.State = source.RewardState;
        }
    }
}