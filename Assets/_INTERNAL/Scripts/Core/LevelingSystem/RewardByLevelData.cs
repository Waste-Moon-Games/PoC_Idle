using UnityEngine;

namespace Core.LevelingSystem
{
    [System.Serializable]
    public class RewardByLevelData
    {
        [field: SerializeField] public string RewardId { get; private set; } = "Default layout: reward name_level";
        [field: SerializeField] public int RequiredLevel { get; private set; }
        [field: SerializeField] public float RewardAmount { get; private set; }
        [field: SerializeField] public bool IsReceived { get; private set; }
        [field: SerializeField] public bool IsUnlocked { get; private set; }
        [field: SerializeField] public RewardType Type { get; private set; }
        [field: SerializeField] public RewardState State { get; private set; }

        public void MarkAsUnlocked()
        {
            IsUnlocked = true;
            State = RewardState.Unlocked;
        }

        public void MarkAsRecived()
        {
            IsReceived = true;
            State = RewardState.Recieved;
        }

        public bool CanBeRecieved() => !IsReceived;
        public bool CanBeUnlocked() => !IsUnlocked;
    }
}