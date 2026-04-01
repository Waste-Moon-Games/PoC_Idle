using Common.MVVM;
using R3;
using UI.GameplayMenu.Models.BonusesFromRewardAd;

namespace UI.GameplayMenu.ViewModels.BonusesFromRewardAd
{
    public class BonusItemViewModel : IViewModel
    {
        private BonusItemModel _model;

        public Observable<bool> BonusInfoWindowStateChanged => _model.BonusInfoWindowStateChangedSignal;

        public void BindModel(IModel model)
        {
            _model = model as BonusItemModel;
        }

        public void OpenItemWindow() => _model.OpenItemWindow();
    }
}