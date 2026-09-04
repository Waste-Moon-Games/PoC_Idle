using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Common
{
    public class UILoadingView : MonoBehaviour
    {
        [SerializeField] private RectTransform _logoTransform;
        [SerializeField] private Image _progressBar;
        [SerializeField] private float _loadingAnimationDuration = 0.5f;
        [SerializeField] private float _logoPulseAnimationDuration = 1.0f;

        private Vector2 _originalLogoScale;

        private Tween _logoPulseTween;
        private Tween _progressTween;

        public void ShowLoadingScreen()
        {
            if (_logoTransform != null)
            {
                _originalLogoScale = _logoTransform.localScale;
                _logoPulseTween?.Kill();

                Vector2 targetScale = _logoTransform.localScale * 0.9f;
                _logoPulseTween = _logoTransform
                    .DOScale(targetScale, _logoPulseAnimationDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }

            _progressTween?.Kill();
            gameObject.SetActive(true);
            _progressBar.fillAmount = 0f;
        }

        public void HideLoadingScreen()
        {
            if(_logoTransform != null)
            {
                _logoPulseTween?.Kill();
                _logoTransform.localScale = _originalLogoScale;
            }

            _progressTween?.Kill();
            gameObject.SetActive(false);
            _progressBar.fillAmount = 0f;
        }

        public void SetLoadingProgress(float progress)
        {
            _progressTween?.Kill();
            _progressTween = _progressBar.DOFillAmount(progress, _loadingAnimationDuration);
        }
    }
}