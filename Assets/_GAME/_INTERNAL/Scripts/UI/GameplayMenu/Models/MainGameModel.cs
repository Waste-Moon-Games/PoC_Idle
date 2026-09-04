using Common.MVVM;
using Core.AudioSystemCommon;
using Core.GlobalGameState;
using R3;

namespace UI.GameplayMenu.Models
{
    public class MainGameModel : IModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly Subject<float> _bonusGaugeChangedSignal = new();

        private readonly PlayerState _model;

        private readonly SoundType _clickSoundType = SoundType.Click;

        public Observable<float> BonusGaugeChange => _bonusGaugeChangedSignal.AsObservable();

        public MainGameModel(PlayerState model)
        {
            _model = model;

            _model.BonusesService.BonusGaugeChanged.Subscribe(HandleChangedBonusGauge).AddTo(_disposables);
        }

        /// <summary>
        /// Запросить дефолтные значения бонусной прогрессии игрока
        /// </summary>
        public void RequestDefaultBonusGaugeState() => _model.BonusesService.RequestDefaultBonusGaugeState();

        public void Dispose() => _disposables.Dispose();

        /// <summary>
        /// Клик
        /// </summary>
        public void Click()
        {
            _model.EconomyService.AddCoins();
            _model.BonusesService.Click();
            AudioEventBus.InvokeSoundSignalByType(_clickSoundType);
        }

        private void HandleChangedBonusGauge(float amount) => _bonusGaugeChangedSignal.OnNext(amount);
    }
}