using Common.MVVM;
using Core.AdsSystem;
using Core.Common.Data;
using Core.GlobalGameState.Services;
using R3;
using System.Collections.Generic;
using UnityEngine;

namespace UI.GameplayMenu.Models.BonusesFromRewardAd
{
    public class PlayerRewardedBonusesModel : IModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly AdsSystemContex _adsSystem;
        private readonly PlayerRewardBonusesService _rewardAdsBonusesService;

        private readonly Subject<string> _bonusSelectedSignal = new();

        private readonly List<BonusItemModel> _items = new();

        public IReadOnlyList<BonusItemModel> BonusItemModels => _items.AsReadOnly();

        public Observable<string> BonusSelectedSignal => _bonusSelectedSignal.AsObservable();

        public PlayerRewardedBonusesModel(
            AdsSystemContex adsSystem,
            PlayerRewardBonusesService playerRewardBonusesService)
        {
            _adsSystem = adsSystem;
            _rewardAdsBonusesService = playerRewardBonusesService;
        }

        public void CreateBonusItemModels(List<BonusItemData> bonusItemDatas, bool ruLang)
        {
            if(bonusItemDatas.Count == 0)
            {
                Debug.LogError($"[Player Rewarded Bonuses Model] Source Item Datas list is empty!");
                return;
            }

            foreach (BonusItemData sourceItemData in bonusItemDatas)
            {
                string desc;

                if (ruLang)
                    desc = sourceItemData.RuDescription;
                else
                    desc = sourceItemData.EnDescription;

                var itemModel = new BonusItemModel(sourceItemData, desc);

                _items.Add(itemModel);
            }
        }

        public void Dispose() => _disposables.Dispose();

        public void ShowAd()
        {
            _adsSystem.ShowRewarded(() =>
            {
                _rewardAdsBonusesService.ActiveTemporaryBonus();
            });
        }

        private void HandleSelectedBonusItem(BonusItemModel item) => _bonusSelectedSignal.OnNext(item.Description);
    }
}