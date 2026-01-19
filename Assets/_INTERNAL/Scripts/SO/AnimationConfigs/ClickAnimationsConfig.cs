using UnityEngine;

namespace SO.AnimationConfigs
{
    [CreateAssetMenu(fileName = "ClickAnimationsConfig", menuName = "Configs/Animations/ClickAnimationsConfig")]
    public class ClickAnimationsConfig : ScriptableObject
    {
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public int InitPoolCount { get; private set; }
        [field: SerializeField] public Transform Cointainer { get; private set; }

        [field: SerializeField] public Transform Target { get; private set; }
        [field: SerializeField] public Vector3 TargetScale { get; private set; }
        [field: SerializeField] public Vector3 DefaultScale { get; private set; }
    }
}