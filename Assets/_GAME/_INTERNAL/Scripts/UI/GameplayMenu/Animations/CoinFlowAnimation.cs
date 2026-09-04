using DG.Tweening;
using UnityEngine;

namespace UI.GameplayMenu.Animations
{
    public class CoinFlowAnimation
    {
        private readonly RectTransform _target;
        private readonly float _flowAmplitude;
        private readonly float _animationDuration;

        private Tween _moveTween;
        private Vector2 _startPosition;

        public CoinFlowAnimation(RectTransform target, float flowAmplitude, float animDuration)
        {
            _target = target;
            _flowAmplitude = flowAmplitude;
            _animationDuration = animDuration;

            _startPosition = _target.anchoredPosition;
        }

        public void StartFlow()
        {
            _target.DOKill();

            _moveTween = _target.DOAnchorPosY(_startPosition.y + _flowAmplitude, _animationDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }

        public void StopFlow()
        {
            _target.DOKill();
        }
    }
}