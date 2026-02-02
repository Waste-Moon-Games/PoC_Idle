using UnityEngine;

namespace Core.LevelingSystem
{
    [System.Serializable]
    public class RewardByLevelData
    {
        [field: SerializeField] public int RequiredLevel { get; private set; }
        [field: SerializeField] public float Reward { get; private set; }
        [field: SerializeField] public bool IsResived { get; private set; }

        public void ReciveReward() => IsResived = true;
    }
}