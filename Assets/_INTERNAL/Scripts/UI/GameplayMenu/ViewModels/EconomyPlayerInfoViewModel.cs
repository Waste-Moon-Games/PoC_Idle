using Common.MVVM;
using R3;
using UI.GameplayMenu.Models;

namespace UI.GameplayMenu.ViewModels
{
    public class EconomyPlayerInfoViewModel : IViewModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly Subject<float> _coinsChangeSignal = new();
        private readonly Subject<float> _coinsPerClickSingal = new();

        private readonly Subject<float> _requestCoinsSignal = new();
        private readonly Subject<float> _requestCoinsPerClickSignal = new();

        private readonly Subject<float> _coinsPerClickChangeSignal = new();
        private readonly Subject<float> _passiveIncomeChangeSignal = new();

        private EconomyPlayerInfoModel _model;

        public Observable<float> CoinsChanged => _coinsChangeSignal.AsObservable();
        public Observable<float> CoinsPerClick => _coinsPerClickSingal.AsObservable();
        public Observable<float> RequestedCoins => _requestCoinsSignal.AsObservable();
        public Observable<float> RequestedCoinsPerClick => _requestCoinsPerClickSignal.AsObservable();
        public Observable<float> CoinsPerClickChanged => _coinsPerClickChangeSignal.AsObservable();
        public Observable<float> PassiveIncomeChanged => _passiveIncomeChangeSignal.AsObservable();

        public void BindModel(IModel model)
        {
            _model = model as EconomyPlayerInfoModel;

            _model.CoinsChanged.Subscribe(HandleChangedCoins).AddTo(_disposables);
            _model.CoinsPerClick.Subscribe(HandleCoinsPerClick).AddTo(_disposables);
            _model.RequestedCoins.Subscribe(HandleRequestedCoinsCount).AddTo(_disposables);
            _model.CoinsPerClickChanged.Subscribe(HandleChangedCoinsPerClick).AddTo(_disposables);
            _model.RequestedCoinsPerClick.Subscribe(HandleRequestedCoinsPerClick).AddTo(_disposables);
            _model.PassiveIncomeChanged.Subscribe(HandleRequestedCurrentPassiveIncome).AddTo(_disposables);
        }

        public void RequestCurrentCoinsCount() => _model.RequestCurrentCoinsCount();
        public void RequestCurrentCoinsPerClick() => _model.RequestCurrentCoinsPerClick();
        public void RequestCurrentPassiveIncome() => _model.RequestCurrentPassiveIncome();

        public void Dispose()
        {
            _model.Dispose();
            _disposables.Dispose();
        }

        private void HandleChangedCoins(float amount) => _coinsChangeSignal.OnNext(amount);
        private void HandleChangedCoinsPerClick(float amount) => _coinsPerClickChangeSignal.OnNext(amount);
        private void HandleRequestedCoinsCount(float amount) => _requestCoinsSignal.OnNext(amount);
        private void HandleRequestedCoinsPerClick(float amount) => _requestCoinsPerClickSignal.OnNext(amount);
        private void HandleCoinsPerClick(float amount) => _coinsPerClickSingal.OnNext(amount);
        private void HandleRequestedCurrentPassiveIncome(float amount) => _passiveIncomeChangeSignal.OnNext(amount);
    }
}