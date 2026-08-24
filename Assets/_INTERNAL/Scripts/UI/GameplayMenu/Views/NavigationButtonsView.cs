using Common.MVVM;
using UI.Common.Components;
using UI.GameplayMenu.ViewModels;
using UnityEngine;

namespace UI.GameplayMenu.Views
{
    public class NavigationButtonsView : MonoBehaviour, IView
    {
        [SerializeField] private ActionButton _shopButton;
        [SerializeField] private ActionButton _settingsButton;

        private NavigationButtonsViewModel _viewModel;

        private void Start()
        {
            if (_shopButton == null || _settingsButton == null)
                return;

            _shopButton.OnButtonClick += HandleShopClick;
            _settingsButton.OnButtonClick += HandleSettingsClick;
        }

        private void OnDestroy()
        {
            if (_shopButton == null || _settingsButton == null)
                return;

            _shopButton.OnButtonClick -= HandleShopClick;
            _settingsButton.OnButtonClick -= HandleSettingsClick;
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as NavigationButtonsViewModel;
        }

        private void HandleShopClick() => _viewModel.ClickShop();
        private void HandleSettingsClick() => _viewModel.ClickSettings();
    }
}