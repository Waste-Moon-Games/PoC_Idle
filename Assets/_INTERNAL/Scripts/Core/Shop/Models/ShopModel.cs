using Common.MVVM;
using Core.GlobalGameState;
using Core.GlobalGameState.Services;
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

        private readonly Dictionary<int, ItemModel> _itemsDict = new();
        private readonly ShopItemsConfig _itemsConfig;
        private readonly ItemsTypeMultiplierConfig _itemsMultiplierConfig;
        private readonly PlayerUpgradeService _model;
        private readonly ShopState _shopState;

        private bool _state;

        public string ShopId => _sId;

        public Observable<Dictionary<int, ItemModel>> RequestedAvailableItems => _requestAvailableItemsSignal.AsObservable();
        public Observable<bool> StateChange => _stateChangedSignal.AsObservable();

        public ShopModel(PlayerUpgradeService model, ShopState shopState, string sId, string configPath, string multiplierConfigPath, bool state)
        {
            _sId = sId;

            _itemsConfig = Resources.Load<ShopItemsConfig>(configPath);
            _itemsMultiplierConfig = Resources.Load<ItemsTypeMultiplierConfig>(multiplierConfigPath);
            _model = model;
            _shopState = shopState;

            _model.SuccessfulPurchase
                .Where(info => info.Item2 == _sId)
                .Subscribe(info => HandleSuccessfulPurchase(info.Item1))
                .AddTo(_disposables);
            _model.FailedPurchase.Subscribe(HandleFailedPurchased).AddTo(_disposables);

            _state = state;
        }
        
        public void InitializeItems()
        {
            InitItemsById(_shopState.HasPurchased(_sId), _shopState.GetPurchasedItems(_sId));

            List<ItemModel> items = _itemsDict.Values.ToList();
            _shopState.InitAvailableItems(items, _sId);
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

        private void InitItemsById(bool shopItemsState, IReadOnlyDictionary<int, ItemModel> itemsDict)
        {
            if (shopItemsState)
            {
                foreach (var kpv in itemsDict)
                    _itemsDict[kpv.Key] = kpv.Value;
            }
            else
            {
                foreach (var itemConfig in _itemsConfig.Items)
                {
                    var itemModel = new ItemModel(itemConfig);
                    _itemsDict[itemModel.Id] = itemModel;
                }
            }
        }

        private void HandleBuyItem(ItemModel item)
        {
            _model.TryUpgradePlayer(item.Price, item.UpgradeAmount, item.Id, item.Type, _sId);
            _shopState.AddPurchasedItemById(item, _sId);
            _shopState.MarkAsPurchased(_sId);
        }

        private void HandleSuccessfulPurchase(int itemId)
        {
            if(!_itemsDict.TryGetValue(itemId, out var item))
            {
                Debug.Log($"Purchased item ID {itemId} not found in shop {_sId}");
                return;
            }

            var priceMultiplier = _itemsMultiplierConfig.UpgradesPriceMultipliers[itemId];
            var upgradeAmountMultiplier = _itemsMultiplierConfig.UpgradesMultipliers[itemId];

            item.IncreasePrice(priceMultiplier);
            item.IncreaseUpgradeAmount(upgradeAmountMultiplier);
            item.IncreaseLevel();

            _shopState.TryOpenNextItem(itemId, _sId);
        }

        private void HandleFailedPurchased(int itemId) => Debug.Log($"Item with info {itemId} cannot was bought");
    }
}