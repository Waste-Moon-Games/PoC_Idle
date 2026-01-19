using Common.MVVM;
using UI.ShopMenu.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ShopMenu.Views
{
    public class NavigationButtonsView : MonoBehaviour, IView
    {
        [Header("Buttons")]
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _clickUpgradesButton;
        [SerializeField] private Button _passiveUpgradesButton;
        [SerializeField] private Button _prestigeUpgradesButton;

        private NavigationButtonsViewModel _viewModel;

        private void Start()
        {
            if (_exitButton == null || _clickUpgradesButton == null || _passiveUpgradesButton == null || _prestigeUpgradesButton == null)
                return;

            _exitButton.onClick.AddListener(HandleExitButtonClick);
            _clickUpgradesButton.onClick.AddListener(HandleClickUpgradesButtonClick);
            _passiveUpgradesButton.onClick.AddListener(HandlePassiveUpgradesButtonClick);
            _prestigeUpgradesButton.onClick.AddListener(HandlePrestigeUpgradesButtonClick);
        }

        private void OnDestroy()
        {
            if (_exitButton == null || _clickUpgradesButton == null || _passiveUpgradesButton == null || _prestigeUpgradesButton == null)
                return;

            _exitButton.onClick.RemoveListener(HandleExitButtonClick);
            _clickUpgradesButton.onClick.RemoveListener(HandleClickUpgradesButtonClick);
            _passiveUpgradesButton.onClick.RemoveListener(HandlePassiveUpgradesButtonClick);
            _prestigeUpgradesButton.onClick.RemoveListener(HandlePrestigeUpgradesButtonClick);
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as NavigationButtonsViewModel;
        }

        private void HandleExitButtonClick() => _viewModel.CloseShop();
        private void HandleClickUpgradesButtonClick() => _viewModel.OpenClickUpgrades();
        private void HandlePassiveUpgradesButtonClick() => _viewModel.OpenPassiveUpgrades();
        private void HandlePrestigeUpgradesButtonClick() => _viewModel.OpenPrestigeUpgrades();

    }
}