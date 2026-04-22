using Core.AudioSystemCommon;
using Core.Common.Player;
using Core.Shop.Base;
using R3;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.GlobalGameState.Services
{
    public class PlayerUpgradeService
    {
        private readonly Dictionary<(ItemType, CurrencyType), Action<float>> _upgradeActions;

        private readonly Subject<(int, string)> _successfulPurchaseSignal = new();
        private readonly Subject<(int, string)> _failedPurchaseSignal = new();

        private readonly PlayerEconomyService _playerEconomyService;
        private readonly PlayerBonusesService _playerBonusesService;
        private readonly AudioSystemService _audioSystemService;

        public Observable<(int, string)> SuccessfulPurchase => _successfulPurchaseSignal.AsObservable();
        public Observable<(int, string)> FailedPurchase => _failedPurchaseSignal.AsObservable();

        public PlayerUpgradeService(PlayerEconomyService playerEconomyService, PlayerBonusesService playerBonusesService, AudioSystemService audioSystemService)
        {
            _playerEconomyService = playerEconomyService;
            _playerBonusesService = playerBonusesService;
            _audioSystemService = audioSystemService;

            _upgradeActions = new()
            {
                { (ItemType.Click, CurrencyType.Coins), IncreasePlayerClick },
                { (ItemType.Click, CurrencyType.Gems), IncreasePlayerExpPerClick },
                { (ItemType.Chance, CurrencyType.Coins), IncreasePlayerTripleChanceClick },
                { (ItemType.Chance, CurrencyType.Gems), IncreasePlayerGemsRewardPerClickChance },
                { (ItemType.Bonus, CurrencyType.Gems), IncreasePlayerBonusPerClick },
                { (ItemType.Passive, CurrencyType.Coins), IncreasePlayerPassiveIncome },
            };
        }

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
                _audioSystemService.PlaySoundByID(SoundsIds.FBuy);
                _failedPurchaseSignal.OnNext((itemId, shopId));
                return;
            }

            if (_upgradeActions.TryGetValue((itemType, currencyType), out var action))
                action?.Invoke(amount);
            else
                Debug.LogWarning($"[Player Upgrade Service] Handler for {itemType} + {currencyType} not found!");

            _audioSystemService.PlaySoundByID(SoundsIds.SBuy);
            _successfulPurchaseSignal.OnNext((itemId, shopId));
        }

        private void IncreasePlayerClick(float amount) => _playerEconomyService.IncreasePlayerClick(amount);
        private void IncreasePlayerExpPerClick(float amount) => _playerBonusesService.TryIncreaseExpPerClick(amount);
        private void IncreasePlayerTripleChanceClick(float amount) => _playerEconomyService.IncreaseTripleClickChance(amount);
        private void IncreasePlayerGemsRewardPerClickChance(float amount) => _playerEconomyService.IncreaseGemsRewardClickChance(amount);
        private void IncreasePlayerBonusPerClick(float amount) => _playerBonusesService.TryIncreaseBonusPerClick(amount);
        private void IncreasePlayerPassiveIncome(float amount) => _playerEconomyService.IncreasePlayerPassiveIncome(amount);
    }
}