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

        private readonly HashSet<string> _hasPurchasedByShop = new();

        public IReadOnlyDictionary<string, ShopModel> ShopModels => _shopsDict;

        public ShopState(PlayerUpgradeService playerUpgradeService)
        {
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
        }

        public List<ShopStateData> Capture()
        {
            return _shopsDict.Select(kv => new ShopStateData()
            {
                ShopID = kv.Key,
                Items = kv.Value.ItemsDict.Select(items => items.Value.Capture()).ToList()
            }).ToList();
        }

        public void Restore(List<ShopStateData> datas)
        {
            for (int i = 0; i < datas.Count; i++)
            {

            }
        }

        public void InitSubscribes()
        {
            foreach (var model in _shopsDict.Values)
                foreach (var item in model.ItemsDict.Values)
                    item.Purchased.Subscribe().AddTo(_disposables);
        }

        public void Dispose() => _disposables.Clear();

        private void InitAvailableItemsById(List<ItemModel> items, Dictionary<int, ItemModel> itemsDict)
        {
            foreach (var item in items)
                itemsDict[item.Id] = item;

            var sortedIds = itemsDict.Keys.OrderBy(x => x).ToList();

            for (int id = 1; id < sortedIds.Count; id++)
            {
                int currentId = sortedIds[id];
                int prevId = sortedIds[id - 1];

                if (itemsDict.TryGetValue(prevId, out var prevItem) &&
                    prevItem != null &&
                    itemsDict.TryGetValue(currentId, out var currentItem))
                {
                    if (prevItem.Level > 0 && prevItem.Level % 5 == 0 && prevItem.IsOpened)
                        currentItem.ChangeStatus(true);
                }
            }
        }
    }
}