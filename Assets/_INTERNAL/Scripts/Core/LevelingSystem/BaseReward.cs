using Core.Consts;
using UnityEngine;

namespace Core.LevelingSystem
{
    public abstract class BaseReward
    {
        [Tooltip("For Reward ID use this layout: rewardName_level_rewardAmount")]
        [SerializeField] protected string ID;
        [SerializeField] protected int RequiredLevel;
        [SerializeField] protected float Amount;
        [SerializeField] protected RewardType Type;
        [SerializeField] protected RewardState State = RewardState.Locked;

        public bool IsReceived => State is RewardState.Received;
        public bool IsUnlocked => State is RewardState.Unlocked;

        public virtual bool TryToUnlock()
        {
            if (State != RewardState.Locked)
                return false;

            State = RewardState.Unlocked;
            return true;
        }

        public virtual bool TryToReceive()
        {
            if (State is not RewardState.Unlocked)
                return false;

            State = RewardState.Received;
            return true;
        }

        public virtual bool CanBeUnlocked() => State == RewardState.Locked;
        public virtual bool CanBeReceived() => State == RewardState.Unlocked;
    }
}