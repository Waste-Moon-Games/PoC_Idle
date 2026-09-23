using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameplayMenu.Animations
{
    public class CoinGlowPulseAnimation
    {
        private readonly RectTransform _target;

        private readonly float _pulseDuration;

        private readonly float _minAlpha;
        private readonly float _maxAlpha;

        private readonly float _minScale;
        private readonly float _maxScale;

        private Sequence _pulseSequence;

        public CoinGlowPulseAnimation(
            RectTransform target, 
            float pulseDuration,
            float minAlpha,
            float maxAlpha, 
            float minScale, 
            float maxScale)
        {
            _target = target;

            _pulseDuration = pulseDuration;

            _minAlpha = minAlpha;
            _maxAlpha = maxAlpha;

            _minScale = minScale;
            _maxScale = maxScale;
        }

        public void StartPulse()
        {
            _pulseSequence?.Kill();

            _pulseSequence = DOTween.Sequence();
            var glowImg = _target.GetComponent<Image>();

            _pulseSequence.Append(_target.DOScale(_maxScale, _pulseDuration).SetEase(Ease.InOutSine));
            _pulseSequence.Join(glowImg.DOFade(_maxAlpha, _pulseDuration));

            _pulseSequence.Append(_target.DOScale(_minScale, _pulseDuration).SetEase(Ease.InOutSine));
            _pulseSequence.Join(glowImg.DOFade(_minAlpha, _pulseDuration));

            _pulseSequence.SetLoops(-1, LoopType.Yoyo);
            _pulseSequence.SetUpdate(true);
        }

        public void StopPulse() => _pulseSequence.Kill();
    }
}