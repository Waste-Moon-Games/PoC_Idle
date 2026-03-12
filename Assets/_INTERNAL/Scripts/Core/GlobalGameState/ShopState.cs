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

            ShopConfigsDatabase configsDatabase = Resources.Load<ShopConfigsDatabase>("Configs/Shop/ShopConfigsDatabase");

            CreateShopModels(playerUpgradeService, configsDatabase);
        }

        private void CreateShopModels(PlayerUpgradeService playerUpgradeService, ShopConfigsDatabase configsDatabase)
        {
            ShopModel clickShopModel = new(
                playerUpgradeService.SuccessfulPurchase,
                playerUpgradeService.FailedPurchase,
                ShopIds.CLICK_UPGRADES,
                true,
                configsDatabase.GetItemsConfigByID(ShopIds.CLICK_UPGRADES),
                configsDatabase.GetRatesConfigByID(ShopIds.CLICK_UPGRADES));

            _shopsDict[clickShopModel.ShopId] = clickShopModel;

            ShopModel passiveShopModel = new(
                playerUpgradeService.SuccessfulPurchase,
                playerUpgradeService.FailedPurchase,
                ShopIds.PASSIVE_UPGRADES,
                false,
                configsDatabase.GetItemsConfigByID(ShopIds.PASSIVE_UPGRADES),
                configsDatabase.GetRatesConfigByID(ShopIds.PASSIVE_UPGRADES));

            _shopsDict[passiveShopModel.ShopId] = passiveShopModel;

            ShopModel prestigeShopModel = new(
                playerUpgradeService.SuccessfulPurchase,
                playerUpgradeService.FailedPurchase,
                ShopIds.PRESTIGE_UPGRADES,
                false,
                configsDatabase.GetItemsConfigByID(ShopIds.PRESTIGE_UPGRADES),
                configsDatabase.GetRatesConfigByID(ShopIds.PRESTIGE_UPGRADES));

            _shopsDict[prestigeShopModel.ShopId] = prestigeShopModel;

            foreach (var shop in _shopsDict.Values)
            {
                shop.PurchaseSignal.Subscribe(s => HandleBuyItem(s.Item1, s.Item2)).AddTo(_disposables);
            }
        }

        public List<ShopStateData> Capture()
        {
            return _shopsDict.Select(shop => new ShopStateData
            {
                ShopID = shop.Key,
                Items = shop.Value.ItemsDict.ToDictionary(
                    item => item.Key,
                    item => item.Value.Capture())
            }).ToList();
        }

        public void Restore(List<ShopStateData> datas)
        {
            var configsDatabase = Resources.Load<ShopConfigsDatabase>("Configs/Shop/ShopConfigsDatabase");

            foreach (var shopData in datas)
            {
                var shopConfig = configsDatabase.GetItemsConfigByID(shopData.ShopID);

                if (shopConfig == null)
                {
                    Debug.LogWarning($"Config not found for shop {shopData.ShopID}");
                    continue;
                }

                var loadedItems = new List<ItemModel>();

                foreach (var configItem in shopConfig.Items)
                {
                    if (!shopData.Items.TryGetValue(configItem.ID, out var savedItem))
                    {
                        savedItem = new ItemUpgradeData
                        {
                            ID = configItem.ID,
                            Level = 0,
                            IsOpened = configItem.IsOpened
                        };
                    }

                    loadedItems.Add(new ItemModel(configItem, savedItem));
                }

                _shopsDict[shopData.ShopID].InitializeItems(true, loadedItems);
            }
        }

        public void InitSubscribes()
        {
            foreach (var model in _shopsDict.Values)
                foreach (var item in model.ItemsDict.Values)
                    item.Purchased.Subscribe().AddTo(_disposables);
        }

        public void Dispose() => _disposables.Clear();

        private void HandleBuyItem(ItemModel item, string shopID)
        {
            _playerUpgradeService.TryUpgradePlayer(item.Price, item.UpgradeAmount, item.Id, item.Type, shopID);
        }
    }
}