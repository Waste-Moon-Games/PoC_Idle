using Common.MVVM;
using Core.GlobalGameState;
using R3;

namespace UI.GameplayMenu.Models
{
    public class PlayerStatsModel : IModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly Subject<int> _levelChangedSingal = new();
        private readonly Subject<int> _currentExpChangedSignal = new();
        private readonly Subject<int> _expToLevelUpChangedSignal = new();

        private PlayerState _model;

        public Observable<int> LevelChanged => _levelChangedSingal.AsObservable();
        public Observable<int> CurrentExpChanged => _currentExpChangedSignal.AsObservable();
        public Observable<int> ExpToLevelUpChanged => _expToLevelUpChangedSignal.AsObservable();

        public void BindModel(PlayerState model)
        {
            _model = model;

            _model.BonusesService.LevelChanged.Subscribe(HandleChangedLevel).AddTo(_disposables);
            _model.BonusesService.CurrentExpChanged.Subscribe(HandleChangedCurrentExp).AddTo(_disposables);
            _model.BonusesService.ExpToLevelUpChanged.Subscribe(HandleChangedExpToLevelUp).AddTo(_disposables);
        }

        /// <summary>
        /// Запросить дефолтные значения уровневой прогрессии игрока
        /// </summary>
        public void RequestDefaultLevelState() => _model.BonusesService.RequestDefaultLevelState();

        public void Dispose() => _disposables.Dispose();

        private void HandleChangedLevel(int level) => _levelChangedSingal.OnNext(level);
        private void HandleChangedCurrentExp(int currentExp) => _currentExpChangedSignal.OnNext(currentExp);
        private void HandleChangedExpToLevelUp(int expToLevelUp) => _expToLevelUpChangedSignal.OnNext(expToLevelUp);
    }
}