using Core.AudioSystemCommon.MonoObjects;
using UnityEngine;

namespace Core.MonoContainers
{
    public class UISFXMonoContainer : MonoBehaviour
    {
        [field: SerializeField] public AudioPlayer AudioPlayer { get; private set; }
    }
}