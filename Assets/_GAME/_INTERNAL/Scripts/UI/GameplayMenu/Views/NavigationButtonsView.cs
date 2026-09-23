using Common.MVVM;
using UI.Common.Components;
using UI.GameplayMenu.ViewModels;
using UnityEngine;

namespace UI.GameplayMenu.Views
{
    public class NavigationButtonsView : MonoBehaviour, IView
    {
        [SerializeField] private ActionButton _shopButton;
        [SerializeField] private ActionButton _wheelOfLuckButton;
        [SerializeField] private ActionButton _settingsButton;

        [SerializeField] private ActionButtonsAnimationsService _buttonsAnimationService;

        private NavigationButtonsViewModel _viewModel;

        private void Start()
        {
            if (_shopButton == null || _settingsButton == null || _wheelOfLuckButton == null)
            {
                Debug.LogError($"[Navigation Buttons View] Some button ref is null!");
                return;
            }

            _shopButton.OnButtonClick += HandleShopClick;
            _wheelOfLuckButton.OnButtonClick += HandleWheelOfLuckClick;
            _settingsButton.OnButtonClick += HandleSettingsClick;

            _buttonsAnimationService.StartAsyncWaveAnimation().Forget();
        }

        private void OnDestroy()
        {
            if (_shopButton == null || _settingsButton == null || _wheelOfLuckButton == null)
            {
                Debug.LogError($"[Navigation Buttons View] Some button ref is null!");
                return;
            }

            _shopButton.OnButtonClick -= HandleShopClick;
            _wheelOfLuckButton.OnButtonClick -= HandleWheelOfLuckClick;
            _settingsButton.OnButtonClick -= HandleSettingsClick;
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as NavigationButtonsViewModel;
        }

        private void HandleShopClick() => _viewModel.ClickShop();
        private void HandleWheelOfLuckClick() => _viewModel.ClickWheelOfLuck();
        private void HandleSettingsClick() => _viewModel.ClickSettings();
    }
}