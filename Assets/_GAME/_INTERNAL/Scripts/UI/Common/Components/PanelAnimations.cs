using System;
using DG.Tweening;
using UnityEngine;

namespace UI.Common.Components
{
    public abstract class PanelAnimations : MonoBehaviour
    {
        [Header("Object Reference")]
        [SerializeField] protected RectTransform _objectRectTransform;

        [Space(5), Header("Animation Durations Setup")]
        [SerializeField] private float _moveAppearAnimationDuration = 1f;
        [SerializeField] private float _scaleAppearAnimationDuration = 1f;
        [SerializeField] protected float _fadeAppearAnimationDuration = 0.75f;

        [Space(5), Header("Move Appear Animation Setup")]
        [SerializeField] private Vector2 _targetPosition;
        [SerializeField] private Ease _inEase = Ease.OutBack;
        [SerializeField] private Ease _outEase = Ease.InBack;

        [Space(5), Header("Fade Appear Animation Setup")]
        [SerializeField] protected CanvasGroup _targetCanvasGroup;

        private Vector2 _originalPosition;

        private Tween _moveAppearTween;
        private Tween _fadeAppearTween;

        public float FadeAnimationDuration => _fadeAppearAnimationDuration;

        protected virtual void Initialize()
        {
            if(_objectRectTransform == null)
                return;

            _originalPosition = _objectRectTransform.anchoredPosition;
        }

        void OnDestroy()
        {
            _moveAppearTween?.Kill();
        }

        public void MoveAppearAnimation()
        {
            gameObject.SetActive(true);
            _moveAppearTween?.Kill();

            _moveAppearTween = _objectRectTransform
                .DOAnchorPos(_targetPosition, _moveAppearAnimationDuration)
                .SetEase(_inEase)
                .OnComplete(() => _objectRectTransform.anchoredPosition = _targetPosition);
        }

        public void MoveDisappearAnimation()
        {
            _moveAppearTween?.Kill(true);

            float disappearDuration = _moveAppearAnimationDuration * 0.5f;

            _moveAppearTween = _objectRectTransform
                .DOAnchorPos(_originalPosition, disappearDuration)
                .SetEase(_outEase)
                .OnComplete(() =>
                {
                    _objectRectTransform.anchoredPosition = _originalPosition;
                    gameObject.SetActive(false);
                });
        }

        /// <summary>
        /// Use for appear and disappear animations
        /// </summary>
        /// <param name="targetAlpha"> 0-1f </param>
        /// <param name="onComplete"> Callback on animation complete, can be null </param>
        public void FadeAnimation(float targetAlpha, Action onComplete = null)
        {
            _fadeAppearTween?.Kill();

            _fadeAppearTween = _targetCanvasGroup
                .DOFade(targetAlpha, _fadeAppearAnimationDuration)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}