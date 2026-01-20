using Common.MVVM;
using Core.GlobalGameState;
using R3;

namespace UI.GameplayMenu.Models
{
    public class MainGameModel : IModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly Subject<float> _bonusGaugeChangedSignal = new();

        private readonly PlayerState _model;

        public Observable<float> BonusGaugeChange => _bonusGaugeChangedSignal.AsObservable();

        public MainGameModel(PlayerState model)
        {
            _model = model;

            _model.BonusGaugeChanged.Subscribe(HandleChangedBonusGauge).AddTo(_disposables);
        }

        public void Dispose() => _disposables.Dispose();

        /// <summary>
        /// Добавить монеты
        /// </summary>
        public void Click()
        {
            _model.EconomyService.Add();
            _model.Click();
        }

        private void HandleChangedBonusGauge(float amount) => _bonusGaugeChangedSignal.OnNext(amount);
    }
}