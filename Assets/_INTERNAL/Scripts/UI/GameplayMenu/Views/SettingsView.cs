using Common.MVVM;
using R3;
using UI.GameplayMenu.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameplayMenu.Views
{
    public class SettingsView : MonoBehaviour, IView
    {
        private readonly CompositeDisposable _disposables = new();

        [Header("Volume sliders")]
        [SerializeField] private Slider _sfxVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;

        [Space(5), Header("Checkboxes")]
        [SerializeField] private Toggle _sfxToggle;
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private GameObject _sfxCheckmark;
        [SerializeField] private GameObject _musicCheckmark;

        [Space(5), Header("Buttons")]
        [SerializeField] private Button _openVK;
        [SerializeField] private Button _closeWindow;

        [Space(5), Header("SFX Sprites")]
        [SerializeField] private Sprite _sfxOnSprite;
        [SerializeField] private Sprite _sfxOffSprite;
        [SerializeField] private Image _sfxIcon;

        [Space(5), Header("Music Sprites")]
        [SerializeField] private Sprite _musicOnSprite;
        [SerializeField] private Sprite _musicOffSprite;
        [SerializeField] private Image _musicIcon;

        private SettingsViewModel _viewModel;

        private void Start()
        {
#if UNITY_WEBGL
            _openVK.gameObject.SetActive(false);
#endif

            _sfxVolumeSlider.onValueChanged.AddListener(ChangeSFXVolume);
            _musicVolumeSlider.onValueChanged.AddListener(ChangeMusicVolume);

            _openVK.onClick.AddListener(HandleOpenVKButtonClick);
            _closeWindow.onClick.AddListener(HandleCloseButtonClick);

            _sfxToggle.onValueChanged.AddListener(ToggleSFXState);
            _musicToggle.onValueChanged.AddListener(ToggleMusicState);
        }

        private void OnDestroy()
        {
            _sfxVolumeSlider.onValueChanged.RemoveListener(ChangeSFXVolume);
            _musicVolumeSlider.onValueChanged.RemoveListener(ChangeMusicVolume);

            _openVK.onClick.RemoveListener(HandleOpenVKButtonClick);
            _closeWindow.onClick.RemoveListener(HandleCloseButtonClick);

            _sfxToggle.onValueChanged.RemoveListener(ToggleSFXState);
            _musicToggle.onValueChanged.RemoveListener(ToggleMusicState);

            _viewModel?.Dispose();
            _disposables.Dispose();
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as SettingsViewModel;

            _viewModel.SettingsWindowStateChangedSignal.Subscribe(HandleChangedWindowState).AddTo(_disposables);
            _viewModel.SFXVolumeChangedSignal.Subscribe(HandleChangedSFXVolume).AddTo(_disposables);
            _viewModel.MusicVolumeChangedSignal.Subscribe(HandleChangedMusicVolume).AddTo(_disposables);
        }

        private void ChangeSFXVolume(float volume) => _viewModel.SetSFXVolume(volume);
        private void ChangeMusicVolume(float volume) => _viewModel.SetMusicVolume(volume);

        private void ToggleSFXState(bool state)
        {
            _viewModel.ToggleSFXState(state);
            _sfxCheckmark.SetActive(state);

            if (state)
                _sfxIcon.sprite = _sfxOnSprite;
            else
                _sfxIcon.sprite = _sfxOffSprite;
        }

        private void ToggleMusicState(bool state)
        {
            _viewModel.ToggleMusicState(state);
            _musicCheckmark.SetActive(state);

            if (state)
                _musicIcon.sprite = _musicOnSprite;
            else
                _musicIcon.sprite = _musicOffSprite;
        }

        private void HandleChangedWindowState(bool state)
        {
            gameObject.SetActive(state);
        }

        private void HandleChangedSFXVolume(float volume) => _sfxVolumeSlider.value = volume;
        private void HandleChangedMusicVolume(float volume) => _musicVolumeSlider.value = volume;
        private void HandleOpenVKButtonClick() => _viewModel.OpenVK();
        private void HandleCloseButtonClick() => _viewModel.Close();
    }
}