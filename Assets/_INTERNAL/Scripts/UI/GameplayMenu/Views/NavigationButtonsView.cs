using Common.MVVM;
using R3;
using UI.GameplayMenu.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameplayMenu.Views
{
    public class NavigationButtonsView : MonoBehaviour, IView
    {
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _settingsButton;

        private NavigationButtonsViewModel _viewModel;

        private void Start()
        {
            if (_shopButton == null || _settingsButton == null)
                return;

            _shopButton.onClick.AddListener(HandleShopClick);
            _settingsButton.onClick.AddListener(HandleSettingsClick);
        }

        private void OnDestroy()
        {
            if (_shopButton == null || _settingsButton == null)
                return;

            _shopButton.onClick.RemoveListener(HandleShopClick);
            _settingsButton.onClick.RemoveListener(HandleSettingsClick);
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as NavigationButtonsViewModel;
        }

        private void HandleShopClick() => _viewModel.ClickShop();
        private void HandleSettingsClick() => _viewModel.ClickSettings();
    }
}