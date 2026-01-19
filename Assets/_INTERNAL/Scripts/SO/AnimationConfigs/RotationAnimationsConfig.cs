using UnityEngine;

namespace SO.AnimationConfigs
{
    [CreateAssetMenu(fileName = "RotationAnimationsConfig", menuName = "Configs/Animations/RotationAnimationsConfig")]
    public class RotationAnimationsConfig : ScriptableObject
    {
        [field: SerializeField] public float RotationSpeed {  get; private set; }
        [field: SerializeField] public Vector3 RotateDirection { get; private set; }
    }
}