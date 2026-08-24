using Common.MVVM;
using R3;
using UI.Common.Components;
using UI.ShopMenu.ViewModels;
using UnityEngine;

namespace UI.ShopMenu.Views
{
    public class NavigationButtonsView : MonoBehaviour, IView
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        [Header("Buttons")]
        [SerializeField] private ActionButton _exitButton;
        [SerializeField] private ActionButton _clickUpgradesButton;
        [SerializeField] private ActionButton _passiveUpgradesButton;
        [SerializeField] private ActionButton _prestigeUpgradesButton;

        private NavigationButtonsViewModel _viewModel;

        private void Start()
        {
            if (_exitButton == null || _clickUpgradesButton == null || _passiveUpgradesButton == null || _prestigeUpgradesButton == null)
                return;

            _exitButton.OnButtonClick += HandleExitButtonClick;
            _clickUpgradesButton.OnButtonClick += HandleClickUpgradesButtonClick;
            _passiveUpgradesButton.OnButtonClick += HandlePassiveUpgradesButtonClick;
            _prestigeUpgradesButton.OnButtonClick += HandlePrestigeUpgradesButtonClick;
        }

        private void OnDestroy()
        {
            _disposables.Dispose();

            if (_exitButton == null || _clickUpgradesButton == null || _passiveUpgradesButton == null || _prestigeUpgradesButton == null)
                return;

            _exitButton.OnButtonClick -= HandleExitButtonClick;
            _clickUpgradesButton.OnButtonClick -= HandleClickUpgradesButtonClick;
            _passiveUpgradesButton.OnButtonClick -= HandlePassiveUpgradesButtonClick;
            _prestigeUpgradesButton.OnButtonClick -= HandlePrestigeUpgradesButtonClick;
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as NavigationButtonsViewModel;

            _viewModel.ActiveShopSignal.Subscribe(HandleActiveShopSignal).AddTo(_disposables);
        }

        private void HandleExitButtonClick() => _viewModel.CloseShop();

        private void HandleActiveShopSignal(int id)
        {
            switch (id)
            {
                case 0:
                    HandleClickUpgradesButtonClick();
                    break;
                case 1:
                    HandlePassiveUpgradesButtonClick();
                    break;
                case 2:
                    HandlePrestigeUpgradesButtonClick();
                    break;
            }
        }

        private void HandleClickUpgradesButtonClick()
        {
            _viewModel.OpenClickUpgrades();

            _clickUpgradesButton.ToggleGlow(true);
            _passiveUpgradesButton.ToggleGlow(false);
            _prestigeUpgradesButton.ToggleGlow(false);
        }

        private void HandlePassiveUpgradesButtonClick()
        {
            _viewModel.OpenPassiveUpgrades();

            _clickUpgradesButton.ToggleGlow(false);
            _passiveUpgradesButton.ToggleGlow(true);
            _prestigeUpgradesButton.ToggleGlow(false);
        }

        private void HandlePrestigeUpgradesButtonClick()
        {
            _viewModel.OpenPrestigeUpgrades();

            _clickUpgradesButton.ToggleGlow(false);
            _passiveUpgradesButton.ToggleGlow(false);
            _prestigeUpgradesButton.ToggleGlow(true);
        }
    }
}