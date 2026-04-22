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
        private readonly AudioSystemService _audioSystemService;

        public Observable<float> BonusGaugeChange => _bonusGaugeChangedSignal.AsObservable();

        public MainGameModel(PlayerState model, AudioSystemService audioSystemService)
        {
            _model = model;

            _model.BonusesService.BonusGaugeChanged.Subscribe(HandleChangedBonusGauge).AddTo(_disposables);
            _audioSystemService = audioSystemService;
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
            _audioSystemService.PlaySoundByID(SoundsIds.ClickSound);
        }

        private void HandleChangedBonusGauge(float amount) => _bonusGaugeChangedSignal.OnNext(amount);
    }
}