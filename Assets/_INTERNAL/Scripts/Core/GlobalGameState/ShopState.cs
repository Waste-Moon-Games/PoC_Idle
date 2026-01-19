using Core.Shop.Base;
using R3;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.GlobalGameState
{
    public class ShopState
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly Dictionary<string, Dictionary<int, ItemModel>> _purchasedItemsByShop = new();

        private readonly HashSet<string> _hasPurchasedByShop = new();

        public void InitAvailableItems(List<ItemModel> items, string shopId)
        {
            if (!_purchasedItemsByShop.ContainsKey(shopId))
                _purchasedItemsByShop[shopId] = new Dictionary<int, ItemModel>();

            var targetDict = _purchasedItemsByShop[shopId];

            InitAvailableItemsById(items, targetDict);
        }

        public Dictionary<int, ItemModel> GetPurchasedItems(string shopId)
        {
            return _purchasedItemsByShop.TryGetValue(shopId, out var dict) ? dict : new();
        }

        public bool HasPurchased(string shopId) => _hasPurchasedByShop.Contains(shopId);
        public void MarkAsPurchased(string shopId) => _hasPurchasedByShop.Add(shopId);

        public void InitSubscribes()
        {
            foreach (var targetDict in _purchasedItemsByShop.Values)
                foreach (var item in targetDict.Values)
                    item.Purchased.Subscribe().AddTo(_disposables);
        }

        public void Dispose() => _disposables.Clear();

        public void TryOpenNextItem(int itemId, string shopId)
        {
            var targetDict = _purchasedItemsByShop[shopId];
            OpenNextItemById(itemId, targetDict);
        }

        public void AddPurchasedItemById(ItemModel item, string shopId)
        {
            if (!_purchasedItemsByShop.TryGetValue(shopId, out var targetDict))
            {
                targetDict = new();
                _purchasedItemsByShop[shopId] = targetDict;
            }

            targetDict[item.Id] = item;
        }

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

        private void OpenNextItemById(int itemId, Dictionary<int, ItemModel> items)
        {
            if (!items.TryGetValue(itemId, out var prevItem)) return;
            if (!items.TryGetValue(itemId + 1, out var item)) return;

            if (item.IsOpened) return;

            if (prevItem.Level % 5 == 0 && prevItem.IsOpened)
                item.ChangeStatus(true);
        }
    }
}