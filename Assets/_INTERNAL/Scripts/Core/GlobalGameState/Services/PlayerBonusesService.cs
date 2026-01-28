using Cysharp.Threading.Tasks;
using R3;
using SO.PlayerConfigs;
using System;
using System.Threading;
using UnityEngine;

namespace Core.GlobalGameState.Services
{
    public class PlayerBonusesService
    {
        private CancellationTokenSource _cts;

        private readonly Subject<float> _bonusGaugeChangedSignal = new();
        private readonly Subject<int> _levelChangedSignal = new();
        private readonly Subject<int> _currentExpChangedSignal = new();
        private readonly Subject<int> _expToLevelUpChangedSignal = new();

        private float _bonusGauge;
        private float _bonusPerClick;
        private readonly float _decreaseBonusGaugeDelay;

        private readonly float _expIncreaseMultiplier;
        private int _level;
        private int _expPerClick;
        private int _currentExp;
        private int _expToLevelUp;

        public Observable<float> BonusGaugeChanged => _bonusGaugeChangedSignal.AsObservable();
        public Observable<int> LevelChanged => _levelChangedSignal.AsObservable();
        public Observable<int> CurrentExpChanged => _currentExpChangedSignal.AsObservable();
        public Observable<int> ExpToLevelUpChanged => _expToLevelUpChangedSignal.AsObservable();

        public PlayerBonusesService(PlayerConfig config)
        {
            _bonusGauge = config.PlayerBonusGauge;
            _bonusPerClick = config.InitPlayerBonusPerClick;

            _level = 1;
            _currentExp = 0;
            _expToLevelUp = config.InitExpToLevelUp;
            _expPerClick = config.InitPlayerExpPerClick;
            _expIncreaseMultiplier = config.ExpIncreaseMultiplier;
            _decreaseBonusGaugeDelay = config.DecreaseBonusGaugeDelay;
        }

        public void Click()
        {
            _bonusGauge += _bonusPerClick;
            TryIncreaseLevel();
            _currentExp += _expPerClick;

            _bonusGaugeChangedSignal.OnNext(_bonusGauge);
            _currentExpChangedSignal.OnNext(_currentExp);
        }

        public void StartAsyncTasks()
        {
            _cts = new();
            DecreaseBonusGaugeAsync(_cts.Token).Forget();
        }

        public void StopAsyncTasks()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        public void TryIncreaseExpPerClick(float amount) => _expPerClick = Mathf.RoundToInt(_expPerClick + amount);
        public void TryIncreaseBonusPerClick(float amount) => _bonusPerClick += amount;

        private void TryIncreaseLevel()
        {
            if (_currentExp >= _expToLevelUp)
            {
                _currentExp = 0;
                _level++;
                _expToLevelUp = Mathf.RoundToInt(_expToLevelUp * _expIncreaseMultiplier);

                _levelChangedSignal.OnNext(_level);
                _currentExpChangedSignal.OnNext(_currentExp);
                _expToLevelUpChangedSignal.OnNext(_expToLevelUp);
            }
        }

        private async UniTask DecreaseBonusGaugeAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_bonusGauge > 0)
                {
                    _bonusGauge--;
                    _bonusGaugeChangedSignal.OnNext(_bonusGauge);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(_decreaseBonusGaugeDelay), ignoreTimeScale: true, cancellationToken: token);
            }
        }
    }
}