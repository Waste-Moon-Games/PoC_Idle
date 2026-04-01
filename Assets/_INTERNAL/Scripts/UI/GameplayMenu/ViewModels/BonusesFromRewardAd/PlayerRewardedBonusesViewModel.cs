using Common.MVVM;
using R3;
using System.Collections.Generic;
using UI.GameplayMenu.Models.BonusesFromRewardAd;

namespace UI.GameplayMenu.ViewModels.BonusesFromRewardAd
{
    public class PlayerRewardedBonusesViewModel : IViewModel
    {
        private PlayerRewardedBonusesModel _model;

        private readonly List<BonusItemViewModel> _itemViewModels = new();

        public IReadOnlyList<BonusItemViewModel> BonusItemViewModels => _itemViewModels;

        public Observable<string> BonusSelectedSignal => _model.BonusSelectedSignal;

        public void BindModel(IModel model)
        {
            _model = model as PlayerRewardedBonusesModel;

            for (int i = 0; i < _model.BonusItemModels.Count; i++)
            {
                var itemModel = _model.BonusItemModels[i];
                BonusItemViewModel itemViewModel = new();
                itemViewModel.BindModel(itemModel);

                _itemViewModels.Add(itemViewModel);
            }
        }

        public void ShowAd() => _model.ShowAd();
        public void CloseBonusInfoWindow() => _model.CloseBonusInfoWindow();

        public void Dispose() => _model.Dispose();
    }
}