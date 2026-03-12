using Core.GlobalGameState.Services;
using Core.SaveSystemBase;
using Core.SaveSystemBase.Data;
using Core.Shop.Base;
using Core.Shop.Models;
using R3;
using SO.ShopConfigs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.GlobalGameState
{
    public class ShopState : ISaveabale<List<ShopStateData>>
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly Dictionary<string, ShopModel> _shopsDict = new();
        private readonly PlayerUpgradeService _playerUpgradeService;

        public IReadOnlyDictionary<string, ShopModel> ShopModels => _shopsDict;

        public ShopState(PlayerUpgradeService playerUpgradeService)
        {
            _playerUpgradeService = playerUpgradeService;

            var configsDatabase = Resources.Load<ShopConfigsDatabase>("Configs/Shop/ShopConfigsDatabase");
            if (configsDatabase == null)
            {
                Debug.LogError("ShopConfigsDatabase not found in Resources/Configs/Shop.");
                return;
            }

            CreateShopModels(configsDatabase);
        }

        private void CreateShopModels(ShopConfigsDatabase configsDatabase)
        {
            foreach (var shopConfig in configsDatabase.ItemsConfigs)
            {
                if (shopConfig == null || string.IsNullOrWhiteSpace(shopConfig.ShopID))
                    continue;

                var shopModel = new ShopModel(_playerUpgradeService.SuccessfulPurchase, _playerUpgradeService.FailedPurchase, shopConfig);
                _shopsDict[shopModel.ShopId] = shopModel;
                shopModel.PurchaseSignal.Subscribe(s => HandleBuyItem(s.Item1, s.Item2)).AddTo(_disposables);
            }
        }

        public List<ShopStateData> Capture()
        {
            return _shopsDict.Select(shop => new ShopStateData
            {
                ShopID = shop.Key,
                Items = shop.Value.ItemsDict.ToDictionary(item => item.Key, item => item.Value.Capture())
            }).ToList();
        }

        public void Restore(List<ShopStateData> datas)
        {
            if (datas == null || datas.Count == 0)
            {
                foreach (var shop in _shopsDict.Values)
                    shop.InitializeItemsFromConfig();
                return;
            }

            var saveById = datas.ToDictionary(d => d.ShopID, d => d);

            foreach (var shop in _shopsDict.Values)
            {
                if (!saveById.TryGetValue(shop.ShopId, out var savedShop))
                {
                    shop.InitializeItemsFromConfig();
                    continue;
                }

                shop.InitializeItemsFromSave(savedShop.Items ?? new Dictionary<int, ItemUpgradeData>());
            }
        }

        public void Dispose() => _disposables.Dispose();

        private void HandleBuyItem(ItemModel item, string shopID)
        {
            _playerUpgradeService.TryUpgradePlayer(item.Price, item.UpgradeAmount, item.Id, item.Type, item.CurrencyType, shopID);
        }
    }
}
