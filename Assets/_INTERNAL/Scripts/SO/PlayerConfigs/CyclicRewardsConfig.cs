using Core.LevelingSystem;
using System.Collections.Generic;
using UnityEngine;

namespace SO.PlayerConfigs
{
    [CreateAssetMenu(menuName = "Configs/Player/CyclicRewardsConfig", fileName = "CyclicRewardsConfig")]
    public class CyclicRewardsConfig : ScriptableObject
    {
        [field: SerializeField] public List<CyclicReward> CyclicRewards { get; private set; } = new();
        [field: SerializeField] public int CyclicRewardRequiredLevelIncreaseStep { get; private set; }
        [field: SerializeField] public float CyclicRewardAmountIncreaseStep { get; private set; }
    }
}