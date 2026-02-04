using UnityEngine;

namespace Core.LevelingSystem
{
    [System.Serializable]
    public class RewardByLevelData
    {
        [field: SerializeField] public int RequiredLevel { get; private set; }
        [field: SerializeField] public float Reward { get; private set; }
        [field: SerializeField] public bool IsReceived { get; private set; }
        [field: SerializeField] public RewardType Type { get; private set; }
        [field: SerializeField] public RewardState State { get; private set; }

        public void MarkAsRecieved() => IsReceived = true;
        public bool CanBeRecieved() => !IsReceived;
    }
}