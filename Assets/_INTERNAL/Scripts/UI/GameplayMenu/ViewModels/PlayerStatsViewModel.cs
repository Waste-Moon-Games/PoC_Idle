using Common.MVVM;
using R3;
using UI.GameplayMenu.Models;
using UnityEngine;

namespace UI.GameplayMenu.ViewModels
{
    public class PlayerStatsViewModel : IViewModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly Subject<int> _levelChangedSingal = new();
        private readonly Subject<(int, int)> _expChangesSignal = new();

        private int _currentExp;
        private int _expToLevelUp;

        private PlayerStatsModel _model;

        public Observable<int> LevelChanged => _levelChangedSingal.AsObservable();
        public Observable<(int, int)> ExpChanges => _expChangesSignal.AsObservable();

        public void BindModel(IModel model)
        {
            _model = model as PlayerStatsModel;

            _model.LevelChanged.Subscribe(HandleChangedLevel).AddTo(_disposables);
            _model.CurrentExpChanged.Subscribe(HandleChangedCurrentExp).AddTo(_disposables);
            _model.ExpToLevelUpChanged.Subscribe(HandleChangedExpToLevelUp).AddTo(_disposables);
        }

        /// <summary>
        /// Запросить дефолтное состояние уровневой прогрессии игрока
        /// </summary>
        public void RequestDefaultLevelState() => _model.RequestDefaultLevelState();

        public void Dispose()
        {
            _model.Dispose();
            _disposables.Dispose();
        }

        private void HandleChangedLevel(int level) => _levelChangedSingal.OnNext(level);

        private void HandleChangedCurrentExp(int currentExp)
        {
            _currentExp = currentExp;
            _expChangesSignal.OnNext((_currentExp, _expToLevelUp));
        }

        private void HandleChangedExpToLevelUp(int expToLevelUp)
        {
            _expToLevelUp = expToLevelUp;
            _expChangesSignal.OnNext((_currentExp, _expToLevelUp));
        }
    }
}