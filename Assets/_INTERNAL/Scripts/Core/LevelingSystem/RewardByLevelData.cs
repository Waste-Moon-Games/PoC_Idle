using UnityEngine;

namespace Core.LevelingSystem
{
    [System.Serializable]
    public class RewardByLevelData
    {
        [field: SerializeField] public string RewardId { get; private set; } = "Default layout: reward name_level";
        [field: SerializeField] public int RequiredLevel { get; private set; }
        [field: SerializeField] public float RewardAmount { get; private set; }
        [field: SerializeField] public RewardType Type { get; private set; }
        [field: SerializeField] public RewardState State { get; private set; } = RewardState.Locked;

        public bool IsReceived => State is RewardState.Received;
        public bool IsUnlocked => State is RewardState.Unlocked;

        public bool TryToUnlock()
        {
            if(State != RewardState.Locked)
                return false;

            State = RewardState.Unlocked;
            return true;
        }

        public bool TryToRecive()
        {
            if(State is not RewardState.Unlocked)
                return false;

            State = RewardState.Received;
            return true;
        }

        public bool CanBeUnlocked() => State == RewardState.Locked;
        public bool CanBeRecieved() => State == RewardState.Unlocked;
    }
}