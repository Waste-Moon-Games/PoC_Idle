using Common.MVVM;
using R3;
using TMPro;
using UI.ShopMenu.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ShopMenu.Views
{
    public class ItemView : MonoBehaviour, IView
    {
        private readonly CompositeDisposable _disposables = new();

        [SerializeField] private Image _icon;
        [SerializeField] private Image _currencyIcon;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private TextMeshProUGUI _upgradeAmountText;
        [SerializeField] private TextMeshProUGUI _levelText;

        [Space(5)]
        [SerializeField] private Button _buyButton;
        [SerializeField] private GameObject _closedMask;

        private ItemViewModel _viewModel;

        private void Start()
        {
            if (_buyButton == null)
                return;

            _buyButton.onClick.AddListener(HandleBuyButtonClick);

            _viewModel?.RequestBaseInfo();
            _viewModel?.RequestGeneralInfo();
        }

        private void OnDestroy()
        {
            if (_buyButton == null)
                return;

            _buyButton.onClick.RemoveListener(HandleBuyButtonClick);

            _viewModel.Dispose();
            _disposables.Dispose();
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as ItemViewModel;

            _viewModel.RequestedIcon.Subscribe(HandleRequestedIcon).AddTo(_disposables);
            _viewModel.RequestedName.Subscribe(HandleRequestedName).AddTo(_disposables);
            _viewModel.RequestedCurrencyIcon.Subscribe(HandleRequestedCurrencyIcon).AddTo(_disposables);

            _viewModel.PriceChanged.Subscribe(HandleChangedPrice).AddTo(_disposables);
            _viewModel.UpgradeAmountChanged.Subscribe(HandleChangedUpgradeAmount).AddTo(_disposables);
            _viewModel.LevelChanged.Subscribe(HandleChangedLevel).AddTo(_disposables);
            _viewModel.StatusChanged.Subscribe(HandleChangedStatus).AddTo(_disposables);

            _viewModel.RequestBaseInfo();
            _viewModel.RequestGeneralInfo();
        }

        private void HandleRequestedIcon(Sprite icon) => _icon.sprite = icon;

        private void HandleRequestedCurrencyIcon(Sprite icon) => _currencyIcon.sprite = icon;

        private void HandleRequestedName(string name) => _nameText.text = $"<color=black>{name}</color>";

        private void HandleChangedPrice(string price) => _priceText.text = $"<color=yellow>{price}</color>";

        private void HandleChangedUpgradeAmount(string finishedDesc) => _upgradeAmountText.text = finishedDesc;

        private void HandleChangedLevel(int level) => _levelText.text = $"<color=green>{level}</color> lvl";

        private void HandleChangedStatus(bool value) => _closedMask.SetActive(!value);

        private void HandleBuyButtonClick() => _viewModel.Buy();
    }
}