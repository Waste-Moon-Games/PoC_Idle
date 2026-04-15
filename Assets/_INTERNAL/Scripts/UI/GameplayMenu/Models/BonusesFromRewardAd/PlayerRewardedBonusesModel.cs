using Common.MVVM;
using Core.AdsSystem;
using Core.Common.Data;
using Core.Enums;
using Core.GlobalGameState.Services;
using R3;
using System.Collections.Generic;
using UnityEngine;
using Utils.Localization;

namespace UI.GameplayMenu.Models.BonusesFromRewardAd
{
    public class PlayerRewardedBonusesModel : IModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly AdsSystemContext _adsSystem;
        private readonly PlayerRewardedBonusesService _rewardAdsBonusesService;

        private readonly Subject<string> _bonusSelectedSignal = new();

        private readonly List<BonusItemModel> _items = new();

        private BonusItemModel _selectedBonus;

        public IReadOnlyList<BonusItemModel> BonusItemModels => _items.AsReadOnly();

        public Observable<string> BonusSelectedSignal => _bonusSelectedSignal.AsObservable();

        public PlayerRewardedBonusesModel(
            AdsSystemContext adsSystem,
            PlayerRewardedBonusesService playerRewardBonusesService)
        {
            _adsSystem = adsSystem;
            _rewardAdsBonusesService = playerRewardBonusesService;

            _rewardAdsBonusesService.TemporaryBonusStateChanged.Subscribe(HandleChangedTemporaryBonusState).AddTo(_disposables);
        }

        public void CreateBonusItemModels(List<BonusItemData> bonusItemDatas, SystemLanguage currentLanguage)
        {
            if(bonusItemDatas.Count == 0)
            {
                Debug.LogError($"[Player Rewarded Bonuses Model] Source Item Datas list is empty!");
                return;
            }

            foreach (BonusItemData sourceItemData in bonusItemDatas)
            {
                var localizedText = sourceItemData.Descriptions;
                string desc = localizedText.Get(currentLanguage);

                var itemModel = new BonusItemModel(sourceItemData, desc);
                itemModel.ItemChosenSignal.Subscribe(HandleSelectedBonusItem).AddTo(_disposables);

                _items.Add(itemModel);
            }
        }

        public void Dispose() => _disposables.Dispose();

        public void ShowAd()
        {
            _adsSystem.ShowRewarded(() =>
            {
                if (_selectedBonus.Type == BonusItemType.TemporaryBonus)
                {
                    _rewardAdsBonusesService.ActiveTemporaryBonus();
                }
                else if (_selectedBonus.Type == BonusItemType.GetCurrencyBonus)
                    _rewardAdsBonusesService.GiveCurrencyBonus(_selectedBonus.Amount);
            });
        }

        public void CloseBonusInfoWindow()
        {
            _selectedBonus.CloseItemWindow();
        }

        private void HandleChangedTemporaryBonusState(bool state)
        {
            if (_selectedBonus.Type == BonusItemType.TemporaryBonus)
                _selectedBonus.SetBonusState(state);
        }

        private void HandleSelectedBonusItem(BonusItemModel item)
        {
            _selectedBonus?.CloseItemWindow();

            _selectedBonus = item;
            _bonusSelectedSignal.OnNext(item.Description);
        }
    }
}