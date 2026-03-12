using Common.MVVM;
using Core.Shop.Base;
using R3;
using SO.ShopConfigs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Shop.Models
{
    public class ShopModel : IModel
    {
        private readonly string _sId;

        private readonly CompositeDisposable _disposables = new();

        private readonly Subject<Dictionary<int, ItemModel>> _requestAvailableItemsSignal = new();
        private readonly Subject<bool> _stateChangedSignal = new();
        private readonly BehaviorSubject<List<ItemModel>> _itemsInitializedSignal;
        private readonly Subject<(ItemModel, string)> _purchaseSignal = new();

        private readonly Dictionary<int, ItemModel> _itemsDict = new();
        private readonly ShopItemsConfig _itemsConfig;
        private readonly ShopRatesConfig _ratesConfig;

        private bool _state;

        public string ShopId => _sId;
        public IReadOnlyDictionary<int, ItemModel> ItemsDict => _itemsDict;

        public Observable<List<ItemModel>> ItemsInitialized => _itemsInitializedSignal.AsObservable();
        public Observable<Dictionary<int, ItemModel>> RequestedAvailableItems => _requestAvailableItemsSignal.AsObservable();
        public Observable<bool> StateChange => _stateChangedSignal.AsObservable();
        public Observable<(ItemModel, string)> PurchaseSignal => _purchaseSignal.AsObservable();

        public ShopModel(Observable<(int, string)> successfulPurchase, Observable<int> failedPurchase, string sId, bool state, ShopItemsConfig itemsConfig, ShopRatesConfig ratesConfig)
        {
            _sId = sId;

            _itemsConfig = itemsConfig;
            _ratesConfig = ratesConfig;

            successfulPurchase
                .Where(info => info.Item2 == _sId)
                .Subscribe(info => HandleSuccessfulPurchase(info.Item1))
                .AddTo(_disposables);
            failedPurchase.Subscribe(HandleFailedPurchased).AddTo(_disposables);

            _state = state;

            var defaultItemsState = new List<ItemModel>();
            for (int i = 0; i < _itemsConfig.Items.Count; i++)
            {
                var item = _itemsConfig.Items[i];
                defaultItemsState.Add(new(item));
            }

            _itemsInitializedSignal = new(defaultItemsState);
        }

        public void InitializeItems(bool saveState, List<ItemModel> items)
        {
            if (saveState)
            {
                for(int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    _itemsDict[item.Id] = item;
                }
            }
            else
            {
                for (int i = 0; i < _itemsConfig.Items.Count; i++)
                {
                    var itemConfig = _itemsConfig.Items[i];
                    var itemModel = new ItemModel(itemConfig);
                    _itemsDict[itemModel.Id] = itemModel;
                }
            }

            _itemsInitializedSignal.OnNext(_itemsDict.Values.ToList());
        }

        public void SubscribeOnItems()
        {
            foreach (var item in _itemsDict.Values)
                item.Purchased.Subscribe(HandleBuyItem).AddTo(_disposables);
        }

        public void RequestState() => _stateChangedSignal.OnNext(_state);

        /// <summary>
        /// Открыть магазин
        /// </summary>
        public void Open()
        {
            _state = true;
            _stateChangedSignal.OnNext(_state);
        }

        /// <summary>
        /// Закрыть магазин
        /// </summary>
        public void Close()
        {
            _state = false;
            _stateChangedSignal.OnNext(_state);
        }

        public void Dispose() => _disposables.Dispose();

        public void RequestItems() => _requestAvailableItemsSignal.OnNext(_itemsDict);

        private void TryOpenNextItem(int itemId)
        {
            OpenNextItemById(itemId, _itemsDict);
        }

        private void OpenNextItemById(int itemId, Dictionary<int, ItemModel> items)
        {
            if (!items.TryGetValue(itemId, out var prevItem)) return;
            if (!items.TryGetValue(itemId + 1, out var item)) return;

            if (item.IsOpened) return;

            if (prevItem.Level % 5 == 0 && prevItem.IsOpened)
                item.ChangeStatus(true);
        }

        private void HandleBuyItem(ItemModel item)
        {
            _purchaseSignal.OnNext((item, ShopId));
            Debug.Log($"Item {item.Name} purchased in {ShopId}");
        }

        private void HandleSuccessfulPurchase(int itemId)
        {
            if(!_itemsDict.TryGetValue(itemId, out var item))
            {
                Debug.Log($"Purchased item ID {itemId} not found in shop {_sId}");
                return;
            }

            ItemRateEntry itemRates = _ratesConfig.Rates.FirstOrDefault(ir => ir.ItemID == item.Name);
            float priceRate = itemRates.PriceRate;
            float upgradeAmountRate = itemRates.BonusRate;

            item.IncreasePrice(priceRate);
            item.IncreaseUpgradeAmount(upgradeAmountRate);
            item.IncreaseLevel();

            TryOpenNextItem(itemId);
        }

        private void HandleFailedPurchased(int itemId) => Debug.Log($"Item with info {itemId} cannot was bought");
    }
}