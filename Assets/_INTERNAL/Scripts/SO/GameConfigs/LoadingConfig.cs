using UnityEngine;

namespace SO.GameConfigs
{
    [CreateAssetMenu(menuName = "Game Configs/Loading System/Loading Config", fileName = "LoadingConfig")]
    public class LoadingConfig : ScriptableObject
    {
        [field: SerializeField] public float MinLoadingTime { get; private set; } = 2.5f;
    }
}