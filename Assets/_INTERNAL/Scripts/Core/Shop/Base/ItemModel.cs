using Common.MVVM;
using Core.Common.Player;
using Core.SaveSystemBase.Data;
using R3;
using System;
using UnityEngine;

namespace Core.Shop.Base
{
    public class ItemModel : IModel
    {
        private readonly Subject<float> _priceChangeSignal = new();
        private readonly Subject<int> _levelChangeSignal = new();
        private readonly Subject<float> _upgradeAmountChangeSignal = new();
        private readonly Subject<bool> _statusChangeSignal = new();
        private readonly Subject<bool> _maxedChangeSignal = new();
        private readonly BehaviorSubject<string> _finalDescriptionSignal;

        private readonly Subject<int> _requestIdSignal = new();
        private readonly Subject<string> _requestNameSignal = new();
        private readonly Subject<Sprite> _requestIconSignal = new();
        private readonly Subject<ItemModel> _tryPurchaseSignal = new();
        private readonly Subject<ItemType> _requestItemTypeSignal = new();
        private readonly Subject<Sprite> _requestedCurrencyIconSignal = new();

        private readonly ItemModelConfig _config;
        private readonly string _description;
        private readonly string _name;

        private bool _isOpened;
        private float _price;
        private float _upgradeAmount;
        private int _level;
        private Sprite _currencyIcon;

        public string Name => _name;
        public string Description => _description;
        public Sprite CurrencyIcon => _currencyIcon;
        public int Id => _config.ID;
        public ItemType Type => _config.Type;
        public CurrencyType CurrencyType => _config.CurrencyType;
        public float PriceRate => _config.PriceRate;
        public float BonusRate => _config.BonusRate;
        public bool HasLevelCap => _config.UseLevelCap;
        public int MaxLevel => _config.MaxLevel;
        public bool IsMaxed => HasLevelCap && Level >= MaxLevel;

        public int Level
        {
            get => _level;
            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Level));

                _level = value;
                _levelChangeSignal.OnNext(_level);
            }
        }

        public bool IsOpened
        {
            get => _isOpened;
            private set
            {
                _isOpened = value;
                _statusChangeSignal.OnNext(_isOpened);
            }
        }

        public float Price
        {
            get => _price;
            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Price));

                _price = value;
                _priceChangeSignal.OnNext(_price);
            }
        }

        public float UpgradeAmount
        {
            get => _upgradeAmount;
            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(UpgradeAmount));

                _upgradeAmount = value;
                _upgradeAmountChangeSignal.OnNext(_upgradeAmount);
            }
        }

        public Observable<float> PriceChanged => _priceChangeSignal.AsObservable();
        public Observable<int> LevelChanged => _levelChangeSignal.AsObservable();
        public Observable<float> UpgradeAmountChanged => _upgradeAmountChangeSignal.AsObservable();
        public Observable<bool> StatusChanged => _statusChangeSignal.AsObservable();
        public Observable<bool> MaxedChanged => _maxedChangeSignal.AsObservable();

        public Observable<int> RequestesId => _requestIdSignal.AsObservable();
        public Observable<string> RequestedName => _requestNameSignal.AsObservable();
        public Observable<string> FinalDescriptionSignal => _finalDescriptionSignal.AsObservable();
        public Observable<Sprite> RequestedIcon => _requestIconSignal.AsObservable();
        public Observable<ItemModel> Purchased => _tryPurchaseSignal.AsObservable();
        public Observable<ItemType> RequestedItemType => _requestItemTypeSignal.AsObservable();
        public Observable<Sprite> RequestedCurrencyIconSignal => _requestedCurrencyIconSignal.AsObservable();

        public ItemModel(ItemModelConfig config, string desc = null, string name = null)
        {
            _config = config;
            _description = desc;
            _name = name;
            _finalDescriptionSignal = new(desc);

            IsOpened = config.IsOpenedByDefault;
            Price = config.StartPrice;
            UpgradeAmount = config.StartUpgradeAmount;
            Level = config.StartLevel;

            _currencyIcon = config.CommonCurrencyIcon;

            if (config.CurrencyType == CurrencyType.Gems)
                _currencyIcon = config.GemsCurrencyIcon;

            if (config.ID == 0)
                ChangeStatus(true);
        }

        public ItemModel(ItemModelConfig config, ItemUpgradeData loadedData, string desc = null, string name = null) : this(config, desc, name)
        {
            Restore(loadedData);
        }

        public ItemUpgradeData Capture()
        {
            return new ItemUpgradeData
            {
                ContentRevision = ItemUpgradeData.CurrentContentRevision,

                ID = Id,
                Name = Name,
                Description = _description,

                Price = Price,
                UpgradeAmount = UpgradeAmount,
                Level = Level,

                IsOpened = IsOpened,
            };
        }

        public void Restore(ItemUpgradeData loadedData)
        {
            if (loadedData == null)
                return;

            int normalizedLevel = Mathf.Max(loadedData.Level, _config.StartLevel);
            bool requiersContentMigration = loadedData.ContentRevision < ItemUpgradeData.CurrentContentRevision;

            if (requiersContentMigration)
            {
                int appliedUpgradeCount = normalizedLevel - _config.StartLevel;
                Price = _config.StartPrice * Mathf.Pow(_config.PriceRate, appliedUpgradeCount);
                UpgradeAmount = _config.StartUpgradeAmount * Mathf.Pow(_config.BonusRate, appliedUpgradeCount);
            }
            else
            {
                Price = loadedData.Price;
                UpgradeAmount = loadedData.UpgradeAmount;
            }

            Level = normalizedLevel;
            IsOpened = loadedData.IsOpened;
        }

        public void RequestBaseInfo()
        {
            _requestIdSignal.OnNext(Id);
            _requestNameSignal.OnNext(Name);
            _requestIconSignal.OnNext(_config.Icon);
            _requestItemTypeSignal.OnNext(Type);
            _requestedCurrencyIconSignal.OnNext(CurrencyIcon);
            _finalDescriptionSignal.OnNext(Description);
        }

        public void RequestGeneralInfo()
        {
            _statusChangeSignal.OnNext(IsOpened);
            _priceChangeSignal.OnNext(Price);
            _levelChangeSignal.OnNext(Level);
            _upgradeAmountChangeSignal.OnNext(UpgradeAmount);
            _finalDescriptionSignal.OnNext(_description);
            _maxedChangeSignal.OnNext(IsMaxed);
        }

        public void ChangeStatus(bool value) => IsOpened = value;
        public void TryBuy()
        {
            if (IsMaxed)
                return;

            _tryPurchaseSignal.OnNext(this);
        }
        public void IncreasePrice(float value) => Price *= value;
        public void IncreaseUpgradeAmount(float value) => UpgradeAmount *= value;
        public void IncreaseLevel()
        {
            if (IsMaxed)
                return;

            Level++;
            _maxedChangeSignal.OnNext(IsMaxed);
        }

        public bool IsValidSaveFor(ItemUpgradeData loadedData) => loadedData != null && loadedData.ID == Id;
    }
}
