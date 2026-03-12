using Common.MVVM;
using Core.SaveSystemBase.Data;
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
        private CompositeDisposable _itemsDisposables = new();

        private readonly Subject<Dictionary<int, ItemModel>> _requestAvailableItemsSignal = new();
        private readonly Subject<bool> _stateChangedSignal = new();
        private readonly BehaviorSubject<List<ItemModel>> _itemsInitializedSignal = new(new List<ItemModel>());
        private readonly Subject<(ItemModel, string)> _purchaseSignal = new();

        private readonly Dictionary<int, ItemModel> _itemsDict = new();
        private readonly ShopItemsConfig _itemsConfig;

        private bool _state;

        public string ShopId => _sId;
        public bool IsOpened => _state;
        public IReadOnlyDictionary<int, ItemModel> ItemsDict => _itemsDict;

        public Observable<List<ItemModel>> ItemsInitialized => _itemsInitializedSignal.AsObservable();
        public Observable<Dictionary<int, ItemModel>> RequestedAvailableItems => _requestAvailableItemsSignal.AsObservable();
        public Observable<bool> StateChange => _stateChangedSignal.AsObservable();
        public Observable<(ItemModel, string)> PurchaseSignal => _purchaseSignal.AsObservable();

        public ShopModel(Observable<(int, string)> successfulPurchase, Observable<(int, string)> failedPurchase, ShopItemsConfig itemsConfig)
        {
            _sId = itemsConfig.ShopID;
            _itemsConfig = itemsConfig;
            _state = itemsConfig.OpenedByDefault;

            successfulPurchase
                .Where(info => info.Item2 == _sId)
                .Subscribe(info => HandleSuccessfulPurchase(info.Item1))
                .AddTo(_disposables);

            failedPurchase
                .Where(info => info.Item2 == _sId)
                .Subscribe(HandleFailedPurchased)
                .AddTo(_disposables);

            InitializeItemsFromConfig();
        }

        public void InitializeItemsFromConfig()
        {
            _itemsDisposables.Dispose();
            _itemsDisposables = new CompositeDisposable();
            _itemsDict.Clear();

            foreach (var itemConfig in _itemsConfig.Items)
            {
                if (itemConfig == null)
                    continue;

                var model = new ItemModel(itemConfig);
                _itemsDict[model.Id] = model;
            }

            SubscribeOnItems();
            _itemsInitializedSignal.OnNext(_itemsDict.Values.OrderBy(i => i.Id).ToList());
        }

        public void InitializeItemsFromSave(Dictionary<int, ItemUpgradeData> savedItems)
        {
            _itemsDisposables.Dispose();
            _itemsDisposables = new CompositeDisposable();
            _itemsDict.Clear();

            foreach (var itemConfig in _itemsConfig.Items)
            {
                if (itemConfig == null)
                    continue;

                savedItems.TryGetValue(itemConfig.ID, out var savedItem);
                var model = savedItem != null ? new ItemModel(itemConfig, savedItem) : new ItemModel(itemConfig);
                _itemsDict[model.Id] = model;
            }

            SubscribeOnItems();
            _itemsInitializedSignal.OnNext(_itemsDict.Values.OrderBy(i => i.Id).ToList());
        }

        public void RequestState() => _stateChangedSignal.OnNext(_state);

        public void Open()
        {
            _state = true;
            _stateChangedSignal.OnNext(_state);
        }

        public void Close()
        {
            _state = false;
            _stateChangedSignal.OnNext(_state);
        }

        public void Dispose()
        {
            _itemsDisposables.Dispose();
            _disposables.Dispose();
        }

        public void RequestItems() => _requestAvailableItemsSignal.OnNext(_itemsDict);

        private void SubscribeOnItems()
        {
            foreach (var item in _itemsDict.Values)
                item.Purchased.Subscribe(HandleBuyItem).AddTo(_itemsDisposables);
        }

        private void TryOpenNextItem(int itemId)
        {
            if (!_itemsDict.TryGetValue(itemId, out var prevItem))
                return;

            if (!_itemsDict.TryGetValue(itemId + 1, out var nextItem))
                return;

            if (nextItem.IsOpened)
                return;

            if (prevItem.Level > 0 && prevItem.Level % 5 == 0)
                nextItem.ChangeStatus(true);
        }

        private void HandleBuyItem(ItemModel item)
        {
            _purchaseSignal.OnNext((item, ShopId));
        }

        private void HandleSuccessfulPurchase(int itemId)
        {
            if (!_itemsDict.TryGetValue(itemId, out var item))
                return;

            item.IncreasePrice(item.PriceRate);
            item.IncreaseUpgradeAmount(item.BonusRate);
            item.IncreaseLevel();

            TryOpenNextItem(itemId);
        }

        private void HandleFailedPurchased((int, string) failedPurchase)
        {
            Debug.Log($"Purchase failed in {_sId}. Item id: {failedPurchase.Item1}");
        }
    }
}
