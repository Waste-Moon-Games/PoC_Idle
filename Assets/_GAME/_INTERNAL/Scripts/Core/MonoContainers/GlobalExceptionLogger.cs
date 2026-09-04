using UnityEngine;

namespace Core.MonoContainers
{
	public class GlobalExceptionLogger : MonoBehaviour
	{
        private void OnEnable()
        {
            Application.logMessageReceived += HandleReceivedLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleReceivedLog;
        }

        private void HandleReceivedLog(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
                Debug.Log($"[FULL TRACE]\n{condition}\n{stackTrace}");
        }
    }
}