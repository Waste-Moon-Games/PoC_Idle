using UnityEngine;

namespace Core.LevelingSystem
{
    [System.Serializable]
    public class RewardByLevel : BaseReward
    {
        public int RewardRequiredLevel => RequiredLevel;
        public float RewardAmount => base.Amount;
        public RewardState RewardState => base.State;
    }
}