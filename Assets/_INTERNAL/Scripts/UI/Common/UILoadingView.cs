using UnityEngine;
using UnityEngine.UI;

namespace UI.Common
{
    public class UILoadingView : MonoBehaviour
    {
        [SerializeField] private Image _progressBar;

        public void ShowLoadingScreen() => gameObject.SetActive(true);
        public void HideLoadingScreen() => gameObject.SetActive(false);
        public void SetLoadingProgress(float progress) => _progressBar.fillAmount = progress;
    }
}