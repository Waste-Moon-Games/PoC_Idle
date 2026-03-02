using Common.MVVM;
using Core.GlobalGameState.Services;
using R3;

namespace UI.GameplayMenu.Models
{
    public class EconomyPlayerInfoModel : IModel
    {
        private readonly CompositeDisposable _disposables = new();

        private PlayerEconomyService _model;

        #region Wallets Observables
        public Observable<float> CoinsChanged => _model.CoinsWalletChanged.AsObservable();
        public Observable<float> GemsChanged => _model.GemsWalletChanged.AsObservable();
        #endregion

        #region UI Observables
        public Observable<float> CoinsPerClickChanged => _model.PlayerClickAmountChanged.AsObservable();
        public Observable<float> CoinsClickAd => _model.CoinsClickAd.AsObservable();
        public Observable<float> PassiveIncomeChanged => _model.PassiveIncomeChanged.AsObservable();
        #endregion

        public void BindModel(PlayerEconomyService model)
        {
            _model = model;
        }
        
        public void Dispose() => _disposables.Dispose();
    }
}