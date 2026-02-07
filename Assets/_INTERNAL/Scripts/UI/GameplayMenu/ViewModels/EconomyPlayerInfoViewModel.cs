using Common.MVVM;
using R3;
using UI.GameplayMenu.Models;

namespace UI.GameplayMenu.ViewModels
{
    public class EconomyPlayerInfoViewModel : IViewModel
    {
        private readonly CompositeDisposable _disposables = new();

        private EconomyPlayerInfoModel _model;

        public Observable<float> CoinsChanged => _model.CoinsChanged.AsObservable();
        public Observable<float> CoinsClickAd => _model.CoinsClickAd.AsObservable();
        public Observable<float> CoinsPerClickChanged => _model.CoinsPerClickChanged.AsObservable();
        public Observable<float> PassiveIncomeChanged => _model.PassiveIncomeChanged.AsObservable();

        public void BindModel(IModel model)
        {
            _model = model as EconomyPlayerInfoModel;
        }

        public void Dispose()
        {
            _model.Dispose();
            _disposables.Dispose();
        }
    }
}