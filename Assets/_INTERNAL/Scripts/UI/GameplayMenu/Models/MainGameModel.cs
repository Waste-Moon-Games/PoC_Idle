using Common.MVVM;
using Core.GlobalGameState;
using R3;
using UnityEngine;

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

            _model.BonusesService.BonusGaugeChanged.Subscribe(HandleChangedBonusGauge).AddTo(_disposables);

            
        }

        /// <summary>
        /// Запросить дефолтные значения бонусной прогрессии игрока
        /// </summary>
        public void RequestDefaultBonusGaugeState() => _model.BonusesService.RequestDefaultBonusGaugeState();

        public void Dispose() => _disposables.Dispose();

        /// <summary>
        /// Добавить монеты
        /// </summary>
        public void Click()
        {
            _model.EconomyService.Add();
            _model.BonusesService.Click();
        }

        private void HandleChangedBonusGauge(float amount) => _bonusGaugeChangedSignal.OnNext(amount);
    }
}