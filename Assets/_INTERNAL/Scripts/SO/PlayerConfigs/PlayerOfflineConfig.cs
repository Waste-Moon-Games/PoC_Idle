using UnityEngine;

namespace SO.PlayerConfigs
{
    [CreateAssetMenu(menuName = "Configs/Player/Player Offline Config", fileName = "PlayerOfflineConfig")]
    public class PlayerOfflineConfig : ScriptableObject
    {
        [field: SerializeField] public int MaxOfflineHours { get; private set; }
    }
}