using DG.Tweening;
using System;
using UnityEngine;

namespace UI.Common.Components
{
    [Serializable]
    public class ButtonAnimations
    {
        [Header("Object Pulse Animation Setup")]
        [SerializeField] private float _objecPusleAnimationDuration = 1.0f;
        [SerializeField] private Vector2 _objecPulseScale = Vector2.one;
        [SerializeField] private bool _useObjectPulsing = false;
        [SerializeField] private float _objectPulseDelay = 0f;

        [Space(5), Header("Glow Pulse Animation Setup")]
        [SerializeField] private float _glowPulseAnimationDuration = 1.0f;
        [SerializeField] private Vector2 _glowPulseScale = Vector2.one;
        [SerializeField] private bool _useGlowPulsing = false;
        [SerializeField] private float _glowPulseDelay = 0f;

        [Space(5), Header("Click Animation Setup")]
        [SerializeField] private float _clickAnimationDuration = 0.15f;
        [SerializeField] private Vector2 _clickedScale = Vector2.one;
        [SerializeField] private float _clickedRandomRotation = 5f;
        [SerializeField] private float _clickedRotationDuration = 0.075f;

        private RectTransform _objectRectTransform;
        private RectTransform _glowRectTransform;

        private Vector3 _defaultScale;

        private Sequence _objectPulseSequence;
        private Sequence _glowPulseSequence;
        private Sequence _clickSequence;

        public bool Initialized => _objectRectTransform != null;

        public void Initialization(RectTransform targetObject, RectTransform glowRectObject = null)
        {
            if(_objectRectTransform == null && targetObject != null)
                _objectRectTransform = targetObject;

            if(_glowRectTransform == null && glowRectObject != null)
                _glowRectTransform = glowRectObject;

            _clickAnimationDuration = 0.1f;

            _defaultScale = _objectRectTransform.localScale;
            StartObjectPulsing();
        }

        public void StartGlowPulsing()
        {
            if (!_useGlowPulsing && _glowRectTransform == null)
                return;

            if (_glowPulseSequence?.IsActive() == true)
                return;

            StopGlowPulsing();

            _glowPulseSequence = DOTween.Sequence();

            _glowPulseSequence
                .PrependInterval(_glowPulseDelay)
                .Append(
                    _glowRectTransform
                        .DOScale(_glowPulseScale, _glowPulseAnimationDuration)
                        .SetEase(Ease.InOutSine))
                .Append(
                    _glowRectTransform
                        .DOScale(_defaultScale, _glowPulseAnimationDuration)
                        .SetEase(Ease.InOutSine))
                .AppendInterval(_glowPulseDelay)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void StartObjectPulsing()
        {
            if (!_useObjectPulsing)
            {
                Debug.LogWarning($"[Button Animations [{_objectRectTransform.gameObject.name}]] didn't use pulsing!");
                return;
            }

            if (_objectPulseSequence?.IsActive() == true)
                return;

            StopObjectPulsing();

            _objectPulseSequence = DOTween.Sequence();

            _objectPulseSequence
                .PrependInterval(_objectPulseDelay)
                .Append(
                    _objectRectTransform
                        .DOScale(_objecPulseScale, _objecPusleAnimationDuration)
                        .SetEase(Ease.InOutSine))
                .Append(
                    _objectRectTransform
                        .DOScale(_defaultScale, _objecPusleAnimationDuration)
                        .SetEase(Ease.InOutSine))
                .AppendInterval(_objectPulseDelay)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void StopObjectPulsing() => _objectPulseSequence?.Kill();

        public void StopGlowPulsing() => _glowPulseSequence?.Kill();

        public void KillAnimations()
        {
            _objectPulseSequence?.Kill();
            _clickSequence?.Kill();
            _glowPulseSequence?.Kill();
        }

        public void ClickAnimation(Action onComplete = null)
        {
            _clickSequence?.Kill();

            _clickSequence = DOTween.Sequence();

            StopObjectPulsing();

            _objectRectTransform.localScale = _defaultScale;

            float pressDuration = _clickAnimationDuration;
            float releaseDuration = _clickAnimationDuration * 2f;
            float randomRotationOffset = UnityEngine.Random.Range(-_clickedRandomRotation, _clickedRandomRotation);
            float clickedRotationDuration = _clickedRotationDuration;
            float clickedRotationReleaseDuration = _clickedRotationDuration * 0.5f;

            _clickSequence.Append(
                _objectRectTransform
                .DOScale(_clickedScale, pressDuration)
                .SetEase(Ease.OutQuart)
                );
            _clickSequence.Append(
                _objectRectTransform
                .DOLocalRotate(new(0f, 0f, randomRotationOffset), clickedRotationDuration)
                );
            _clickSequence.Append(_objectRectTransform
                .DOScale(_defaultScale, releaseDuration)
                .SetEase(Ease.OutBack));
            _clickSequence.Append(
                _objectRectTransform
                .DOLocalRotate(Vector2.zero, clickedRotationReleaseDuration)
                );

            _clickSequence.OnComplete(() =>
            {
                if (_useObjectPulsing)
                    StartObjectPulsing();
                _objectRectTransform.localScale = _defaultScale;
                onComplete?.Invoke();
            });
        }
    }
}