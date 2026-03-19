using Core.Common.Player;
using Core.Shop.Base;
using R3;
using UnityEngine;

namespace Core.GlobalGameState.Services
{
    public class PlayerUpgradeService
    {
        private readonly Subject<(int, string)> _successfulPurchaseSignal = new();
        private readonly Subject<(int, string)> _failedPurchaseSignal = new();

        private readonly PlayerEconomyService _playerEconomyService;

        public Observable<(int, string)> SuccessfulPurchase => _successfulPurchaseSignal.AsObservable();
        public Observable<(int, string)> FailedPurchase => _failedPurchaseSignal.AsObservable();

        public PlayerUpgradeService(PlayerEconomyService playerEconomyService) => _playerEconomyService = playerEconomyService;

        public void TryUpgradePlayer(
            float price,
            float amount,
            int itemId,
            ItemType itemType,
            CurrencyType currencyType,
            string shopId)
        {
            if (!_playerEconomyService.TryToSpend(currencyType, price))
            {
                _failedPurchaseSignal.OnNext((itemId, shopId));
                return;
            }

            switch (itemType)
            {
                case ItemType.Click:
                    _playerEconomyService.IncreasePlayerClick(amount);
                    break;
                case ItemType.Chance:
                    if (currencyType == CurrencyType.Coins)
                        _playerEconomyService.IncreaseTrippleClickChance(amount);
                    else
                        _playerEconomyService.IncreaseGemsRewardClickChance(amount);
                    break;
                case ItemType.Passive:
                    _playerEconomyService.IncreasePlayerPassiveIncome(amount);
                    break;
                case ItemType.Prestige:
                    Debug.Log("Increase prestige");
                    break;
            }

            _successfulPurchaseSignal.OnNext((itemId, shopId));
        }
    }
}
