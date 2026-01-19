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
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _description;
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
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as ItemViewModel;

            _viewModel.RequestedIcon.Subscribe(HandleRequestedIcon).AddTo(_disposables);
            _viewModel.RequestedName.Subscribe(HandleRequestedName).AddTo(_disposables);
            _viewModel.RequestedDescription.Subscribe(HandleRequestedDescription).AddTo(_disposables);

            _viewModel.PriceChanged.Subscribe(HandleChangedPrice).AddTo(_disposables);
            _viewModel.UpgradeAmountChanged.Subscribe(HandleChangedUpgradeAmount).AddTo(_disposables);
            _viewModel.LevelChanged.Subscribe(HandleChangedLevel).AddTo(_disposables);
            _viewModel.StatusChanged.Subscribe(HandleChangedStatus).AddTo(_disposables);

            _viewModel.RequestBaseInfo();
            _viewModel.RequestGeneralInfo();
        }

        private void HandleRequestedIcon(Sprite icon) => _icon.sprite = icon;

        private void HandleRequestedName(string name) => _nameText.text = $"<color=white>{name}</color>";

        private void HandleRequestedDescription(string description) => _description.text = description;

        private void HandleChangedPrice(string price) => _priceText.text = $"<color=yellow>{price}</color>";

        private void HandleChangedUpgradeAmount(string amount) => _upgradeAmountText.text = $"+<color=#FFD600>{amount}</color>";

        private void HandleChangedLevel(int level) => _levelText.text = $"<color=green>{level}</color> lvl";

        private void HandleChangedStatus(bool value) => _closedMask.SetActive(!value);

        private void HandleBuyButtonClick() => _viewModel.Buy();
    }
}