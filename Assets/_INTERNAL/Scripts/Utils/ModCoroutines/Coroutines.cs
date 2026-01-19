using System.Collections;
using UnityEngine;

namespace Utils.ModCoroutines
{
    public sealed class Coroutines : MonoBehaviour
    {
        private static Coroutines _instance;
        private static GameObject _object;

        private static Coroutines Instance
        {
            get
            {
                return _instance;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorCleanup()
        {
            UnityEditor.EditorApplication.playModeStateChanged += change =>
            {
                if (change == UnityEditor.PlayModeStateChange.ExitingPlayMode && _object != null)
                {
                    DestroyImmediate(_object);
                    _instance = null;
                    _object = null;
                }
            };
        }
#endif

        public void Awake()
        {
            if(_instance == null)
                _instance = this;
        }

        private void OnApplicationQuit()
        {
            if (_instance == this)
            {
                DestroyImmediate(gameObject);
                _instance = null;
                _object = null;
            }
        }

        public Coroutine StartRoutine(IEnumerator enumerator)
        {
            if (Instance != null)
            {
                return Instance.StartCoroutine(enumerator);
            }
            return null;
        }

        public void StopRoutine(Coroutine routine)
        {
            if (routine != null && Instance != null && Instance.gameObject != null)
            {
                Instance.StopCoroutine(routine);
            }
        }
    }
}