using Core.AdsSystem;
using Core.Common.Data;
using System.Collections.Generic;
using UnityEngine;

namespace SO.AdsConfigs
{
    [CreateAssetMenu(menuName = "Configs/Ads/Reward Ads Config", fileName = "RewardAdsConfig")]
    public class RewardAdsConfig : ScriptableObject
    {
        [field: SerializeField] public float InitTemporaryBonusDurationInMinutes { get; private set; }

        [field: Space(5), Header("Items Config")]
        [field: SerializeField] public List<BonusItemData> Items { get; private set; }

        [field: Space(5), Header("Ad Placements Setup")]
        [field: SerializeField] public Dictionary<RewardedAdType, string> PlacementsDict { get; private set; } = new();
    }
}