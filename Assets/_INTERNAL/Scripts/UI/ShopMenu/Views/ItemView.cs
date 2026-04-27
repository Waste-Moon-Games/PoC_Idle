using Common.MVVM;
using Core.Common.Player;
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
        private bool _isMaxed;

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
            _viewModel.MaxedChanged.Subscribe(HandleMaxedChanged).AddTo(_disposables);
            _viewModel.FinalDescSignal.Subscribe(HandleFinalDesc).AddTo(_disposables);

            _viewModel.RequestBaseInfo();
            _viewModel.RequestGeneralInfo();
        }

        private void HandleFinalDesc(string desc) => _upgradeAmountText.text = desc;

        private void HandleRequestedIcon(Sprite icon) => _icon.sprite = icon;

        private void HandleRequestedCurrencyIcon(Sprite icon) => _currencyIcon.sprite = icon;

        private void HandleRequestedName(string name) => _nameText.text = $"<color=black>{name}</color>";

        private void HandleChangedPrice(string price)
        {
            if (_isMaxed)
            {
                _priceText.text = "<color=#00E676>MAXED</color>";
                return;
            }

            if (_viewModel.CurrencyType == CurrencyType.Gems)
            {
                _priceText.text = $"<color=red>{price}</color>";
                return;
            } 

            _priceText.text = $"<color=yellow>{price}</color>";
        }

        private void HandleChangedUpgradeAmount(string finalDesc) => _upgradeAmountText.text = finalDesc;

        private void HandleChangedLevel(int level)
        {
            _levelText.text = _isMaxed
                ? "<color=#00E676>MAXED</color>"
                : $"<color=green>{level}</color> lvl";
        }

        private void HandleChangedStatus(bool value) => _closedMask.SetActive(!value);

        private void HandleMaxedChanged(bool value)
        {
            _isMaxed = value;

            _buyButton.interactable = !_isMaxed;

            if (_isMaxed)
            {
                _levelText.text = "<color=#00E676>MAXED</color>";
                _priceText.text = "<color=#00E676>MAXED</color>";
            }
        }

        private void HandleBuyButtonClick() => _viewModel.Buy();
    }
}