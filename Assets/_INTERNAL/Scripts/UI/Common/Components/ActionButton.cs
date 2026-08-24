using Core.AudioSystemCommon;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Common.Components
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Button))]
    public class ActionButton : MonoBehaviour
    {
        [Header("Rect Transforms")]
        [SerializeField] private RectTransform _glowRectTransform;
        [SerializeField] private RectTransform _objectRectTransform;

        [Space(5), Header("Animations")]
        [SerializeField] private ButtonAnimations _animations;

        private readonly SoundType _soundType = SoundType.UI_Click;

        private Button _button;

        public bool Interactable
        {
            get => _button.interactable;
            set
            {
                _button.interactable = value;
            }
        }

        public event Action OnButtonClick;

        private void Awake()
        {
            _objectRectTransform = GetComponent<RectTransform>();

            _button = GetComponent<Button>();
            _button.onClick.AddListener(HandleButtonClick);

            if(_glowRectTransform == null)
            {
                _animations.Initialization(_objectRectTransform);
                return;
            }

            _animations.Initialization(_objectRectTransform, _glowRectTransform);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(HandleButtonClick);

            _animations.KillAnimations();
        }

        public void ForceInit()
        {
            if (_animations.Initialized)
                return;

            _button = GetComponent<Button>();
            _button.onClick.AddListener(HandleButtonClick);

            if (_glowRectTransform == null)
            {
                _animations.Initialization(_objectRectTransform);
                return;
            }

            _animations.Initialization(_objectRectTransform, _glowRectTransform);
        }

        public void ToggleGlow(bool value) => _glowRectTransform.gameObject.SetActive(value);

        public void StartPulsing() => _animations.StartObjectPulsing();
        public void StopPulsing() => _animations.StopObjectPulsing();

        private void HandleButtonClick()
        {
            AudioEventBus.InvokeSoundSignalByType(_soundType);

            Interactable = false;

            _animations.ClickAnimation(() =>
            {
                OnButtonClick?.Invoke();
                Interactable = true;
            });
            _animations.StartGlowPulsing();
        }
    }
}