using UnityEngine;

namespace SO.PlayerConfigs
{
    [CreateAssetMenu(menuName = "Configs/Player/Player Offline Config", fileName = "Player Offline Config")]
    public class PlayerOfflineConfig : ScriptableObject
    {
        [field: SerializeField] public int MaxOfflineSeconds { get; private set; }
    }
}