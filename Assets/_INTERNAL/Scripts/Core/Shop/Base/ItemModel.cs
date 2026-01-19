using Common.MVVM;
using R3;
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
        private readonly Subject<(string, string)> _requestedDescriptionSignal = new();
        private readonly Subject<Sprite> _requestIconSignal = new();
        private readonly Subject<ItemModel> _purchaseSignal = new();
        #endregion

        private readonly int _id;
        private readonly string _name;
        private readonly string _descriptionTemplate;
        private readonly string _description;
        private readonly Sprite _icon;
        private readonly ItemType _itemType;

        private bool _isOpened;
        private float _price;
        private float _upgradeAmount;
        private int _level;

        public string Name => _name;
        public int Id => _id;
        public int Level => _level;
        public ItemType Type => _itemType;
        public bool IsOpened => _isOpened;
        public float Price => _price;
        public float UpgradeAmount => _upgradeAmount;

        #region Changeable Observables
        public Observable<float> PriceChanged => _priceChangeSignal.AsObservable();
        public Observable<int> LevelChanged => _levelChangeSignal.AsObservable();
        public Observable<float> UpgradeAmountChanged => _upgradeAmountChangeSignal.AsObservable();
        public Observable<bool> StatusChanged => _statusChangeSignal.AsObservable();
        #endregion

        #region Non-changeable Observables
        public Observable<int> RequestesId => _requestIdSignal.AsObservable();
        public Observable<string> RequestedName => _requestNameSignal.AsObservable();
        public Observable<(string, string)> RequestedDescription => _requestedDescriptionSignal.AsObservable();
        public Observable<Sprite> RequestedIcon => _requestIconSignal.AsObservable();
        public Observable<ItemModel> Purchased => _purchaseSignal.AsObservable();
        #endregion

        public ItemModel(ItemModelConfig sourceData)
        {
            _id = sourceData.ID;
            _name = sourceData.Name;
            _icon = sourceData.Icon;
            _descriptionTemplate = sourceData.Description;
            _itemType = sourceData.Type;

            _isOpened = sourceData.IsOpened;
            _price = sourceData.Price;
            _upgradeAmount = sourceData.UpgradeAmount;
            _level = sourceData.Level;

            _description = _descriptionTemplate.Replace("{amount}", $"{_upgradeAmount}");
        }

        public void RequestBaseInfo()
        {
            _requestIdSignal.OnNext(_id);
            _requestNameSignal.OnNext(_name);
            _requestIconSignal.OnNext(_icon);
            _requestedDescriptionSignal.OnNext((_description, _descriptionTemplate));
        }

        public void RequestGeneralInfo()
        {
            _statusChangeSignal.OnNext(_isOpened);
            _priceChangeSignal.OnNext(_price);
            _levelChangeSignal.OnNext(_level);
            _upgradeAmountChangeSignal.OnNext(_upgradeAmount);
        }

        public void ChangeStatus(bool value)
        {
            _isOpened = value;
            _statusChangeSignal.OnNext(value);
        }

        public void TryBuy()
        {
            // to do, переписать на _tryPurchaseSignal. Чтобы отправлять попытку, а не покупку
            _purchaseSignal.OnNext(this);
        }

        public void IncreasePrice(float multiplier)
        {
            _price *= multiplier;
            _priceChangeSignal.OnNext(_price);
        }

        public void IncreaseLevel()
        {
            _level++;
            _levelChangeSignal.OnNext(_level);
#if UNITY_EDITOR
            Debug.Log($"[INCREASE] {Name} (ID:{Id}, Inst:{InstanceId}) → Level = {Level}");
#endif
        }

        public void IncreaseUpgradeAmount(float multiplier)
        {
            _upgradeAmount *= multiplier;

            _upgradeAmountChangeSignal.OnNext(_upgradeAmount);
            _requestedDescriptionSignal.OnNext((_description, _descriptionTemplate));
        }
    }
}