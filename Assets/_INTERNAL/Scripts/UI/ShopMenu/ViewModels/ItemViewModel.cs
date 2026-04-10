using Common.MVVM;
using Core.Shop.Base;
using R3;
using UnityEngine;
using Utils.Formatter;

namespace UI.ShopMenu.ViewModels
{
    public class ItemViewModel : IViewModel
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly NumberFormatter _formatter = new();

        private readonly Subject<string> _requestNameSignal = new();
        private readonly Subject<Sprite> _requestIconSignal = new();
        private readonly Subject<Sprite> _requestedCurrencyIconSignal = new();

        private readonly Subject<string> _priceChangeSignal = new();
        private readonly Subject<string> _upgradeAmountChangeSignal = new();
        private readonly Subject<int> _levelChangeSignal = new();
        private readonly Subject<bool> _statusChangeSignal = new();

        private int _id;
        private Sprite _icon;
        private Sprite _currencyIcon;
        private string _name;
        private ItemType _type;
        private string _finalDescription;

        private bool _isOpened;
        private string _price;
        private string _upgradeAmount;
        private int _level;

        private ItemModel _model;

        public ItemType ItemType => _type;
        public string ItemName => _model.Name;

        public Observable<string> RequestedName => _requestNameSignal.AsObservable();
        public Observable<Sprite> RequestedIcon => _requestIconSignal.AsObservable();
        public Observable<Sprite> RequestedCurrencyIcon => _requestedCurrencyIconSignal.AsObservable();

        public Observable<string> PriceChanged => _priceChangeSignal.AsObservable();
        public Observable<string> UpgradeAmountChanged => _upgradeAmountChangeSignal.AsObservable();
        public Observable<int> LevelChanged => _levelChangeSignal.AsObservable();
        public Observable<bool> StatusChanged => _statusChangeSignal.AsObservable();

        public void BindModel(IModel model)
        {
            _model = model as ItemModel;

            _model.RequestesId.Subscribe(HandleRequestedId).AddTo(_disposables);
            _model.RequestedName.Subscribe(HandleRequestedName).AddTo(_disposables);
            _model.RequestedIcon.Subscribe(HandleRequestedIcon).AddTo(_disposables);
            _model.RequestedItemType.Subscribe(HandleRequestedItemType).AddTo(_disposables);
            _model.RequestedCurrencyIconSignal.Subscribe(HandleRequestedCurrencyIcon).AddTo(_disposables);
            _model.FinalDescriptionSignal.Subscribe(HandleFinalDescription).AddTo(_disposables);

            _model.PriceChanged.Subscribe(HandlePriceChanged).AddTo(_disposables);
            _model.LevelChanged.Subscribe(HandleLevelChanged).AddTo(_disposables);
            _model.UpgradeAmountChanged.Subscribe(HandleUpgradeAmountChanged).AddTo(_disposables);
            _model.StatusChanged.Subscribe(HandleStatusChanged).AddTo(_disposables);
        }

        public void Dispose() => _disposables.Dispose();

        public void Buy() => _model.TryBuy();

        public void RequestBaseInfo() => _model.RequestBaseInfo();

        public void RequestGeneralInfo() => _model.RequestGeneralInfo();

        private void HandleRequestedId(int id) => _id = id;

        private void HandleRequestedCurrencyIcon(Sprite icon)
        {
            _currencyIcon = icon;
            _requestedCurrencyIconSignal.OnNext(_currencyIcon);
        }

        private void HandleRequestedName(string name)
        {
            _name = name;
            _requestNameSignal.OnNext(_name);
        }

        private void HandleRequestedIcon(Sprite icon)
        {
            _icon = icon;
            _requestIconSignal.OnNext(_icon);
        }

        private void HandleRequestedItemType(ItemType type) => _type = type;

        private void HandlePriceChanged(float price)
        {
            _price = _formatter.FormatNumber(price);
            _priceChangeSignal.OnNext(_price);
        }

        private void HandleLevelChanged(int level)
        {
            _level = level;
            _levelChangeSignal.OnNext(_level);
        }

        private void HandleStatusChanged(bool value)
        {
            _isOpened = value;
            _statusChangeSignal.OnNext(_isOpened);
        }

        private void HandleFinalDescription(string desc)
        {
            _finalDescription = desc.Replace("{amount}", _upgradeAmount);
            _upgradeAmountChangeSignal.OnNext(_finalDescription);
        }

        private void HandleUpgradeAmountChanged(float amount)
        {
            string oldValue = _upgradeAmount;
            _upgradeAmount = _formatter.FormatNumber(amount);

            _finalDescription = _finalDescription.Replace(oldValue, _upgradeAmount);

            _upgradeAmountChangeSignal.OnNext(_finalDescription);
        }
    }
}