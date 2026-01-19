using Common.MVVM;
using Core.GlobalGameState.Services;
using R3;

namespace UI.GameplayMenu.Models
{
    public class PlayerInfoModel : IModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly Subject<float> _coinsChangeSignal = new();
        private readonly Subject<float> _coinsPerClickSignal = new();

        private readonly Subject<float> _requestCoinsSignal = new();
        private readonly Subject<float> _requestCoinsPerClickSignal = new();

        private readonly Subject<float> _passiveIncomeChangeSignal = new();
        private readonly Subject<float> _coinsPerClickChangeSignal = new();

        private PlayerEconomyService _model;

        public Observable<float> CoinsChanged => _coinsChangeSignal.AsObservable();
        public Observable<float> CoinsPerClick => _coinsPerClickSignal.AsObservable();
        public Observable<float> RequestedCoins => _requestCoinsSignal.AsObservable();
        public Observable<float> RequestedCoinsPerClick => _requestCoinsPerClickSignal.AsObservable();
        public Observable<float> CoinsPerClickChanged => _coinsPerClickChangeSignal.AsObservable();
        public Observable<float> PassiveIncomeChanged => _passiveIncomeChangeSignal.AsObservable();

        public void BindModel(PlayerEconomyService model)
        {
            _model = model;

            _model.CoinsChanged.Subscribe(HandleChangedCoins).AddTo(_disposables);
            _model.CoinsPerClick.Subscribe(HandleCoinsPerClick).AddTo(_disposables);
            _model.CoinsPerClickChanged.Subscribe(HandleChangedCoinsPerClick).AddTo(_disposables);
            _model.PassiveInconeChanged.Subscribe(HandleRequestedCurrentPassiveIncome).AddTo(_disposables);
        }

        public void RequestCurrentCoinsCount() => _requestCoinsSignal.OnNext(_model.PlayerWallet);
        public void RequestCurrentCoinsPerClick() => _requestCoinsPerClickSignal.OnNext(_model.PlayerClickAmount);
        public void RequestCurrentPassiveIncome() => _model.RequestCurrentPassiveIncome();

        public void Dispose() => _disposables.Dispose();

        private void HandleChangedCoins(float amount) => _coinsChangeSignal.OnNext(amount);
        private void HandleChangedCoinsPerClick(float amount) => _coinsPerClickChangeSignal.OnNext(amount);
        private void HandleCoinsPerClick(float amount) => _coinsPerClickSignal.OnNext(amount);
        private void HandleRequestedCurrentPassiveIncome(float amount) => _passiveIncomeChangeSignal.OnNext(amount);
    }
}