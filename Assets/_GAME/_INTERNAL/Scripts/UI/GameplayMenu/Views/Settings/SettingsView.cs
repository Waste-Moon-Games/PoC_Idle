using Common.MVVM;
using Core.AudioSystemCommon;
using DG.Tweening;
using R3;
using UI.Common.Components;
using UI.GameplayMenu.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameplayMenu.Views.Settings
{
    public class SettingsView : MonoBehaviour, IView
    {
        private readonly CompositeDisposable _disposables = new();

        [Header("Volume sliders")]
        [SerializeField] private Slider _sfxVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;

        [Space(5), Header("Buttons")]
        [SerializeField] private ActionButton _openVK;
        [SerializeField] private ActionButton _closeWindow;

        [Space(5), Header("SFX")]
        [SerializeField] private ActionButton _sfxToggler;
        [SerializeField] private Image _sfxTogglerIcon;
        [SerializeField] private Sprite _sfxOnSprite;
        [SerializeField] private Sprite _sfxOffSprite;
        [SerializeField] private Image _sfxIcon;
        [SerializeField] private RectTransform _sfxHandlerRect;

        [Space(5), Header("Music")]
        [SerializeField] private ActionButton _musicToggler;
        [SerializeField] private Image _musicTogglerIcon;
        [SerializeField] private Sprite _musicOnSprite;
        [SerializeField] private Sprite _musicOffSprite;
        [SerializeField] private Image _musicIcon;
        [SerializeField] private RectTransform _musicHandlerRect;

        [Space(5), Header("Vibration Settings")]
        [SerializeField] private Image _vibroIcon;
        [SerializeField] private Image _vibroTogglerIcon;
        [SerializeField] private Sprite _vibroOffSprite;
        [SerializeField] private Sprite _vibroOnSprite;
        [SerializeField] private ActionButton _vibrationsToggler;
        [SerializeField] private RectTransform _vibroHandlerRect;
        
        [Space(5), Header("Other Settings")]
        [SerializeField] private Sprite _onHandlerSprite;
        [SerializeField] private Sprite _offHandlerSprite;

        [Space(5), Header("Animations Setup")]
        [SerializeField] private float _toggleAnimationDuration = 1.25f;
        [SerializeField] private float _toggleHandlerAnimationDuration = 0.15f;
        [SerializeField] private float _onTogglePostionX;

        private readonly SoundType _openSoundType = SoundType.Open;
        private readonly SoundType _closeSoundType = SoundType.Close;

        private Vector2 _originalScale;

        private SettingsViewModel _viewModel;

        private Tween _openTween;
        private Tween _closeTween;

        private Tween _toggleVibrationsTween;
        private Tween _toggleSFXTween;
        private Tween _toggleMusicTween;

        private void Start()
        {
#if UNITY_WEBGL
            _openVK.gameObject.SetActive(false);
#endif
            _originalScale = Vector2.one;

            _sfxVolumeSlider.onValueChanged.AddListener(ChangeSFXVolume);
            _musicVolumeSlider.onValueChanged.AddListener(ChangeMusicVolume);

            _sfxToggler.OnButtonClick += ToggleSFXState;
            _musicToggler.OnButtonClick += ToggleMusicState;
            _vibrationsToggler.OnButtonClick += ToggleVibroState;

            _openVK.OnButtonClick += HandleOpenVKButtonClick;
            _closeWindow.OnButtonClick += HandleCloseButtonClick;

            ToggleSFXState();
            ToggleMusicState();
            ToggleVibroState();
        }

        private void OnDestroy()
        {
            _sfxVolumeSlider.onValueChanged.RemoveListener(ChangeSFXVolume);
            _musicVolumeSlider.onValueChanged.RemoveListener(ChangeMusicVolume);

            _openVK.OnButtonClick -= HandleOpenVKButtonClick;
            _closeWindow.OnButtonClick -= HandleCloseButtonClick;

            _sfxToggler.OnButtonClick -= ToggleSFXState;
            _musicToggler.OnButtonClick -= ToggleMusicState;
            _vibrationsToggler.OnButtonClick -= ToggleVibroState;

            _viewModel?.Dispose();
            _disposables.Dispose();
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as SettingsViewModel;

            InitTogglers();

            _viewModel.SettingsWindowStateChangedSignal.Subscribe(HandleChangedWindowState).AddTo(_disposables);
            _viewModel.SFXVolumeChangedSignal.Subscribe(HandleChangedSFXVolume).AddTo(_disposables);
            _viewModel.MusicVolumeChangedSignal.Subscribe(HandleChangedMusicVolume).AddTo(_disposables);
        }

        private void ChangeSFXVolume(float volume) => _viewModel.SetSFXVolume(volume);
        private void ChangeMusicVolume(float volume) => _viewModel.SetMusicVolume(volume);

        private void InitTogglers()
        {
            bool sfxState = _viewModel.CurrentSFXState;
            bool musicState = _viewModel.CurrentMusicState;
            bool vibroState = _viewModel.CurrentVibroState;

            if (sfxState)
            {
                _sfxHandlerRect.anchoredPosition = new(_onTogglePostionX, _sfxHandlerRect.anchoredPosition.y);
                _sfxIcon.sprite = _sfxOnSprite;
                _sfxTogglerIcon.sprite = _onHandlerSprite;
            }
            else
            {
                _sfxHandlerRect.anchoredPosition = new(-_onTogglePostionX, _sfxHandlerRect.anchoredPosition.y);
                _sfxIcon.sprite = _sfxOffSprite;
                _sfxTogglerIcon.sprite = _offHandlerSprite;
            }

            if (musicState)
            {
                _musicHandlerRect.anchoredPosition = new(_onTogglePostionX, _musicHandlerRect.anchoredPosition.y);
                _musicIcon.sprite = _musicOnSprite;
                _musicTogglerIcon.sprite = _onHandlerSprite;
            }
            else
            {
                _musicHandlerRect.anchoredPosition = new(-_onTogglePostionX, _musicHandlerRect.anchoredPosition.y);
                _musicIcon.sprite = _musicOffSprite;
                _musicTogglerIcon.sprite = _offHandlerSprite;
            }

            if (vibroState)
            {
                _vibroHandlerRect.anchoredPosition = new(_onTogglePostionX, _vibroHandlerRect.anchoredPosition.y);
                _vibroIcon.sprite = _vibroOnSprite;
                _vibroTogglerIcon.sprite = _onHandlerSprite;
            }
            else
            {
                _vibroHandlerRect.anchoredPosition = new(-_onTogglePostionX, _vibroHandlerRect.anchoredPosition.y);
                _vibroIcon.sprite = _vibroOffSprite;
                _vibroTogglerIcon.sprite = _offHandlerSprite;
            }
        }

        private void ToggleSFXState()
        {
            bool state = _viewModel.CurrentSFXState;

            if (state)
            {
                _toggleSFXTween?.Kill();
                _toggleSFXTween = _sfxHandlerRect
                    .DOAnchorPosX(_onTogglePostionX, _toggleHandlerAnimationDuration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        _sfxIcon.sprite = _sfxOnSprite;
                        _sfxTogglerIcon.sprite = _onHandlerSprite;
                    });
            }
            else
            {
                _toggleSFXTween?.Kill();
                _toggleSFXTween = _sfxHandlerRect
                    .DOAnchorPosX(-_onTogglePostionX, _toggleHandlerAnimationDuration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        _sfxIcon.sprite = _sfxOffSprite;
                        _sfxTogglerIcon.sprite = _offHandlerSprite;
                    });
            }

            _viewModel.ToggleSFXState(!state);
        }

        private void ToggleMusicState()
        {
            bool state = _viewModel.CurrentMusicState;

            if (state)
            {
                _toggleMusicTween?.Kill();
                _toggleMusicTween = _musicHandlerRect
                    .DOAnchorPosX(_onTogglePostionX, _toggleHandlerAnimationDuration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        _musicIcon.sprite = _musicOnSprite;
                        _musicTogglerIcon.sprite = _onHandlerSprite;
                    });
            }
            else
            {
                _toggleMusicTween?.Kill();
                _toggleMusicTween = _musicHandlerRect
                    .DOAnchorPosX(-_onTogglePostionX, _toggleHandlerAnimationDuration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        _musicIcon.sprite = _musicOffSprite;
                        _musicTogglerIcon.sprite = _offHandlerSprite;
                    });
            }

            _viewModel.ToggleMusicState(!state);
        }

        private void ToggleVibroState()
        {
            bool state = _viewModel.CurrentVibroState;

            if (state)
            {
                _toggleVibrationsTween?.Kill();
                _toggleVibrationsTween = _vibroHandlerRect
                    .DOAnchorPosX(_onTogglePostionX, _toggleHandlerAnimationDuration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        _vibroIcon.sprite = _vibroOnSprite;
                        _vibroTogglerIcon.sprite = _onHandlerSprite;
                    });
            }
            else
            {
                _toggleVibrationsTween?.Kill();
                _toggleVibrationsTween = _vibroHandlerRect
                    .DOAnchorPosX(-_onTogglePostionX, _toggleHandlerAnimationDuration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        _vibroIcon.sprite = _vibroOffSprite;
                        _vibroTogglerIcon.sprite = _offHandlerSprite;
                    });
            }

            _viewModel.ToggleVibroState(!state);
        }

        private void HandleChangedWindowState(bool state)
        {
            if (state)
            {
                AudioEventBus.InvokeSoundSignalByType(_openSoundType);
                transform.localScale = Vector2.zero;
                gameObject.SetActive(true);

                _openTween?.Kill();
                _closeTween?.Kill();

                _openTween = transform
                    .DOScale(Vector2.one, _toggleAnimationDuration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        transform.localScale = _originalScale;
                    });
            }
            else
            {
                AudioEventBus.InvokeSoundSignalByType(_closeSoundType);

                _openTween?.Kill();
                _closeTween?.Kill();

                _closeTween = transform
                    .DOScale(Vector2.zero, _toggleAnimationDuration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        transform.localScale = Vector2.zero;
                        gameObject.SetActive(false);
                    });
            }
        }

        private void HandleChangedSFXVolume(float volume) => _sfxVolumeSlider.value = volume;
        private void HandleChangedMusicVolume(float volume) => _musicVolumeSlider.value = volume;
        private void HandleOpenVKButtonClick() => _viewModel.OpenVK();
        private void HandleCloseButtonClick() => _viewModel.Close();
    }
}