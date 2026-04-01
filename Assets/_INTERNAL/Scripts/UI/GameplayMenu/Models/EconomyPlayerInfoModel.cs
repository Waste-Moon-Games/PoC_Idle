using Common.MVVM;
using Core.GlobalGameState.Services;
using R3;

namespace UI.GameplayMenu.Models
{
    public class EconomyPlayerInfoModel : IModel
    {
        private readonly CompositeDisposable _disposables = new();

        private PlayerEconomyService _economyModel;
        private PlayerRewardedBonusesService _rewardedBonusesService;

        #region Wallets Observables
        public Observable<float> CoinsChanged => _economyModel.CoinsWalletChanged.AsObservable();
        public Observable<float> GemsChanged => _economyModel.GemsWalletChanged.AsObservable();
        #endregion

        #region UI Observables
        public Observable<float> CoinsPerClickChanged => _economyModel.PlayerClickAmountChanged.AsObservable();
        public Observable<float> CoinsClickAd => _economyModel.CoinsClickAd.AsObservable();
        public Observable<float> PassiveIncomeChanged => _economyModel.PassiveIncomeChanged.AsObservable();
        public Observable<bool> TemporaryBonusStateChanged => _rewardedBonusesService.TemporaryBonusStateChanged.AsObservable();
        public Observable<float> TemporaryBonusTimerChanged => _rewardedBonusesService.TemporaryBonusTimerChanged.AsObservable();
        #endregion

        public void BindModel(PlayerEconomyService model, PlayerRewardedBonusesService playerRewardBonusesService)
        {
            _economyModel = model;
            _rewardedBonusesService = playerRewardBonusesService;
        }
        
        public void Dispose() => _disposables.Dispose();
    }
}