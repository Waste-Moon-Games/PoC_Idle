using Core.LevelingSystem;
using System.Collections.Generic;
using UnityEngine;

namespace SO.PlayerConfigs
{
    [CreateAssetMenu(menuName = "Configs/Player/RewardsByLevelConfig", fileName = "RewardsByLevelConfig")]
    public class RewardsByLevelConfig : ScriptableObject
    {
        [field: SerializeField] public List<RewardByLevelData> RewardsByLevel { get; private set; }
    }
}