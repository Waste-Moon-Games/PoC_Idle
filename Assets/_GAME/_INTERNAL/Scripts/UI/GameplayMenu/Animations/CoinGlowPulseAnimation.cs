using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameplayMenu.Animations
{
    public class CoinGlowPulseAnimation
    {
        private readonly RectTransform _target;
        private readonly float _pulseDuration;
        private readonly float _minScale;
        private readonly float _maxScale;

        public CoinGlowPulseAnimation(RectTransform target, float pulseDuration, float minScale, float maxScale)
        {
            _target = target;
            _pulseDuration = pulseDuration;
            _minScale = minScale;
            _maxScale = maxScale;
        }

        public void StartPulse()
        {
            _target.DOKill();

            Sequence pulseSequence = DOTween.Sequence();
            var glowImg = _target.GetComponent<Image>();

            pulseSequence.Append(_target.DOScale(_maxScale, _pulseDuration * 0.5f).SetEase(Ease.InOutSine));
            pulseSequence.Join(glowImg.DOFade(0.8f, _pulseDuration * 0.5f));

            pulseSequence.Append(_target.DOScale(_minScale, _pulseDuration * 0.5f).SetEase(Ease.InOutSine));
            pulseSequence.Join(glowImg.DOFade(0.2f, _pulseDuration * 0.5f));

            pulseSequence.SetLoops(-1, LoopType.Restart);
            pulseSequence.SetUpdate(true);
        }

        public void StopPulse() => _target.DOKill();
    }
}