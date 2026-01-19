using Core.Shop.Base;
using R3;
using UnityEngine;

namespace Core.GlobalGameState.Services
{
    public class PlayerUpgradeService
    {
        private readonly Subject<(int, string)> _successfulPurchaseSignal = new();
        private readonly Subject<int> _failedPurchaseSignal = new();

        private readonly PlayerEconomyService _playerEconomyService;

        public Observable<(int, string)> SuccessfulPurchase => _successfulPurchaseSignal.AsObservable();
        public Observable<int> FailedPurchase => _failedPurchaseSignal.AsObservable();

        public PlayerUpgradeService(PlayerEconomyService playerEconomyService) => _playerEconomyService = playerEconomyService;

        public void TryUpgradePlayer(float price, float amount, int itemId, ItemType itemType, string shopId)
        {
            if (_playerEconomyService.HasEnoughCoins(price))
            {
                _playerEconomyService.Spend(price);

                switch (itemType)
                {
                    case ItemType.Click:
                        _playerEconomyService.IncreasePlayerClick(amount);
                        break;
                    case ItemType.Chance:
                        _playerEconomyService.IncreaseTrippleClickChance(amount);
                        break;
                    case ItemType.Passive:
                        _playerEconomyService.IncreasePlayerPassiveIncome(amount);
                        break;
                    case ItemType.Prestige:
                        Debug.Log("Increase prestige");
                        break;
                }

                _successfulPurchaseSignal.OnNext((itemId, shopId));

                return;
            }

            _failedPurchaseSignal.OnNext(itemId);
        }
    }
}