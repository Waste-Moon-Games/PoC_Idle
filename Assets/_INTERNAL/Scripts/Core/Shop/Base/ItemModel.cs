using Common.MVVM;
using Core.SaveSystemBase.Data;
using R3;
using System;
using UnityEngine;

namespace Core.Shop.Base
{
    public class ItemModel : IModel
    {
#if UNITY_EDITOR
        private static int _globalCounter = 0;
        public readonly int InstanceId = ++_globalCounter;
#endif

        #region Changeable Subjects
        private readonly Subject<float> _priceChangeSignal = new();
        private readonly Subject<int> _levelChangeSignal = new();
        private readonly Subject<float> _upgradeAmountChangeSignal = new();
        private readonly Subject<bool> _statusChangeSignal = new();
        #endregion

        #region Non-changeable Subjects
        private readonly Subject<int> _requestIdSignal = new();
        private readonly Subject<string> _requestNameSignal = new();
        private readonly Subject<Sprite> _requestIconSignal = new();
        private readonly Subject<ItemModel> _purchaseSignal = new();
        private readonly Subject<ItemType> _requestItemTypeSignal = new();
        #endregion

        private int _id;
        private string _name;
        private readonly Sprite _icon;
        private readonly ItemType _itemType;

        private bool _isOpened;
        private float _price;
        private float _upgradeAmount;
        private int _level;

        public string Name => _name;
        public int Id => _id;
        public int Level
        {
            get => _level;
            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(_level));

                _level = value;
                _levelChangeSignal.OnNext(_level);
            }
        }
        public ItemType Type => _itemType;
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
                    throw new ArgumentOutOfRangeException(nameof(_price));

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
                    throw new ArgumentOutOfRangeException(nameof(_upgradeAmount));

                _upgradeAmount = value;
                _upgradeAmountChangeSignal.OnNext(_upgradeAmount);
            }
        }

        #region Changeable Observables
        public Observable<float> PriceChanged => _priceChangeSignal.AsObservable();
        public Observable<int> LevelChanged => _levelChangeSignal.AsObservable();
        public Observable<float> UpgradeAmountChanged => _upgradeAmountChangeSignal.AsObservable();
        public Observable<bool> StatusChanged => _statusChangeSignal.AsObservable();
        #endregion

        #region Non-changeable Observables
        public Observable<int> RequestesId => _requestIdSignal.AsObservable();
        public Observable<string> RequestedName => _requestNameSignal.AsObservable();
        public Observable<Sprite> RequestedIcon => _requestIconSignal.AsObservable();
        public Observable<ItemModel> Purchased => _purchaseSignal.AsObservable();
        public Observable<ItemType> RequestedItemType => _requestItemTypeSignal.AsObservable();
        #endregion

        public ItemModel(ItemModelConfig sourceData)
        {
            _id = sourceData.ID;
            _name = sourceData.Name;
            _icon = sourceData.Icon;
            _itemType = sourceData.Type;

            IsOpened = sourceData.IsOpened;
            Price = sourceData.Price;
            UpgradeAmount = sourceData.UpgradeAmount;
            Level = sourceData.Level;
        }

        public ItemModel(ItemModelConfig sourceData, ItemUpgradeData loadedData)
        {
            _id = loadedData.ID;
            _name = loadedData.Name;
            _icon = sourceData.Icon;
            _itemType = sourceData.Type;

            IsOpened = loadedData.IsOpened;
            Price = loadedData.Price;
            UpgradeAmount = loadedData.UpgradeAmount;
            Level = loadedData.Level;
        }

        public ItemUpgradeData Capture()
        {
            return new ItemUpgradeData()
            {
                ID = _id,
                Name = _name,
                Price = Price,
                UpgradeAmount = UpgradeAmount,
                IsOpened = _isOpened,
                Level = _level,
            };
        }

        public void Restore(ItemUpgradeData loadedData)
        {
            _id = loadedData.ID;
            _name = loadedData.Name;
            Price = loadedData.Price;
            UpgradeAmount = loadedData.UpgradeAmount;
            Level = loadedData.Level;
            IsOpened = loadedData.IsOpened;
        }

        public void RequestBaseInfo()
        {
            _requestIdSignal.OnNext(_id);
            _requestNameSignal.OnNext(_name);
            _requestIconSignal.OnNext(_icon);
            _requestItemTypeSignal.OnNext(_itemType);
        }

        public void RequestGeneralInfo()
        {
            _statusChangeSignal.OnNext(IsOpened);
            _priceChangeSignal.OnNext(Price);
            _levelChangeSignal.OnNext(Level);
            _upgradeAmountChangeSignal.OnNext(UpgradeAmount);
        }

        public void ChangeStatus(bool value)
        {
            IsOpened = value;
        }

        public void TryBuy()
        {
            // to do, переписать на _tryPurchaseSignal. Чтобы отправлять попытку, а не покупку
            _purchaseSignal.OnNext(this);
        }

        public void IncreasePrice(float multiplier)
        {
            Price *= multiplier;
        }

        public void IncreaseLevel()
        {
            Level++;
#if UNITY_EDITOR
            Debug.Log($"[INCREASE] {Name} (ID:{Id}, Inst:{InstanceId}) → Level = {Level}");
#endif
        }

        public void IncreaseUpgradeAmount(float multiplier)
        {
            UpgradeAmount *= multiplier;
        }
    }
}