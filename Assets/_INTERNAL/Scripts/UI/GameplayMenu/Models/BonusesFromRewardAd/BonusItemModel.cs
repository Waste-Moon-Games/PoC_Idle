using Common.MVVM;
using Core.Common.Data;
using Core.Enums;
using R3;

namespace UI.GameplayMenu.Models.BonusesFromRewardAd
{
    public class BonusItemModel : IModel
    {
        private readonly string _itemID;
        private readonly BonusItemType _type;
        private readonly string _description;
        private readonly float _amount;
        private readonly float _bonusDuration;

        private readonly Subject<BonusItemModel> _itemChosenSignal = new();
        private readonly Subject<bool> _bonusInfoWindowStateChangedSignal = new();
        private readonly Subject<bool> _bonusActiveStateChangedSignal = new();

        private bool _isOpened = false;
        private bool _isActive = false;

        public string ItemID => _itemID;
        public BonusItemType Type => _type;
        public string Description => _description;
        public float Amount => _amount;
        public bool IsActive => _isActive;

        public Observable<BonusItemModel> ItemChosenSignal => _itemChosenSignal.AsObservable();
        public Observable<bool> BonusInfoWindowStateChangedSignal => _bonusInfoWindowStateChangedSignal.AsObservable();
        public Observable<bool> BonusActiveStateChanged => _bonusActiveStateChangedSignal.AsObservable();

        public BonusItemModel(BonusItemData sourceData, string desc = null)
        {
            _itemID = sourceData.ItemID;

            _type = sourceData.Type;
            _amount = sourceData.Amount;
            _bonusDuration = sourceData.BonusDuration;

            _description = desc;
        }

        public void OpenItemWindow()
        {
            _isOpened = true;

            _itemChosenSignal.OnNext(this);
            _bonusInfoWindowStateChangedSignal.OnNext(_isOpened);
        }

        public void SetBonusState(bool state)
        {
            _isActive = state;
            _bonusActiveStateChangedSignal.OnNext(_isActive);
        }

        public void CloseItemWindow()
        {
            _isOpened = false;
            _bonusInfoWindowStateChangedSignal.OnNext(_isOpened);
        }
    }
}