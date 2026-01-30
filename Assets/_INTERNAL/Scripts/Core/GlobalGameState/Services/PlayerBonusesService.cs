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
        private CancellationTokenSource _decreaseBonusCancellationToken;
        private CancellationTokenSource _activeBonusCancellationToken;

        private readonly Subject<bool> _bonusStateChangedSignal = new();

        private readonly Subject<float> _bonusGaugeChangedSignal = new();
        private readonly Subject<int> _levelChangedSignal = new();
        private readonly Subject<int> _currentExpChangedSignal = new();
        private readonly Subject<int> _expToLevelUpChangedSignal = new();

        private float _bonusGauge;
        private bool _isBonusActive = false;
        private readonly float _maxBonusGauge;
        private float _bonusPerClick;
        private readonly float _decreaseBonusGaugeDelay;
        private readonly float _decreaseActiveBonusGaugeDelay;

        private readonly float _expIncreaseMultiplier;
        private int _level;
        private int _gainedExpPerClick;
        private int _currentExp;
        private int _expToLevelUp;

        public Observable<bool> BonusStateChanged => _bonusStateChangedSignal.AsObservable();
        public Observable<float> BonusGaugeChanged => _bonusGaugeChangedSignal.AsObservable();
        public Observable<int> LevelChanged => _levelChangedSignal.AsObservable();
        public Observable<int> CurrentExpChanged => _currentExpChangedSignal.AsObservable();
        public Observable<int> ExpToLevelUpChanged => _expToLevelUpChangedSignal.AsObservable();

        public PlayerBonusesService(PlayerConfig config)
        {
            _maxBonusGauge = config.PlayerBonusGauge;
            _bonusGauge = 0f;
            _bonusPerClick = config.InitPlayerBonusPerClick;

            _level = 1;
            _currentExp = 0;
            _expToLevelUp = config.InitExpToLevelUp;
            _gainedExpPerClick = config.InitGainedPlayerExpPerClick;
            _expIncreaseMultiplier = config.ExpIncreaseMultiplier;

            _decreaseBonusGaugeDelay = config.DecreaseBonusGaugeDelay;
            _decreaseActiveBonusGaugeDelay = config.DecreaseActiveBonusGaugeDelay;
        }

        /// <summary>
        /// Запросить дефолтные значения бонусной прогрессии игрока
        /// </summary>
        public void RequestDefaultBonusGaugeState()
        {
            _bonusGaugeChangedSignal.OnNext(_bonusGauge);
        }

        /// <summary>
        /// Запросить дефолтные значения уровневой прогрессии игрока
        /// </summary>
        public void RequestDefaultLevelState()
        {
            _levelChangedSignal.OnNext(_level);
            _currentExpChangedSignal.OnNext(_currentExp);
            _expToLevelUpChangedSignal.OnNext(_expToLevelUp);
        }

        public void Click()
        {
            if (_bonusGauge < _maxBonusGauge && !_isBonusActive)
            {
                _bonusGauge += _bonusPerClick;
                if(_bonusGauge >= _maxBonusGauge)
                {
                    _isBonusActive = true;
                    _bonusStateChangedSignal.OnNext(_isBonusActive);
                    _bonusGauge = _maxBonusGauge;
                    StartActiveBonusTask();
                }
            }

            TryIncreaseLevel();
            _currentExp += _gainedExpPerClick;

            float currentBonus = _bonusGauge / _maxBonusGauge;
            _bonusGaugeChangedSignal.OnNext(currentBonus);
            _currentExpChangedSignal.OnNext(_currentExp);
        }

        public void StartAsyncDecreaseTask()
        {
            _decreaseBonusCancellationToken = new();
            DecreaseBonusGaugeAsync(_decreaseBonusCancellationToken.Token).Forget();
        }

        public void StopAsyncDecreaseTask()
        {
            _decreaseBonusCancellationToken?.Cancel();
            _decreaseBonusCancellationToken?.Dispose();
            _decreaseBonusCancellationToken = null;
        }

        public void TryIncreaseExpPerClick(float amount) => _gainedExpPerClick = Mathf.RoundToInt(_gainedExpPerClick + amount);
        public void TryIncreaseBonusPerClick(float amount) => _bonusPerClick += amount;

        private void StartActiveBonusTask()
        {
            _activeBonusCancellationToken = new();
            ActiveBonusTaskAsync(_activeBonusCancellationToken.Token).Forget();
            StopAsyncDecreaseTask();
        }

        private void StopActiveBonusTask()
        {
            _activeBonusCancellationToken?.Cancel();
            _activeBonusCancellationToken?.Dispose();
            _activeBonusCancellationToken = null;
        }

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

        private async UniTask ActiveBonusTaskAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_bonusGauge > 0)
                {
                    _bonusGauge--;
                    float currentBonus = _bonusGauge / _maxBonusGauge;
                    _bonusGaugeChangedSignal.OnNext(currentBonus);

                    if (_bonusGauge <= 0)
                    {
                        _bonusGauge = 0;
                        StopActiveBonusTask();
                        StartAsyncDecreaseTask();

                        _isBonusActive = false;
                        _bonusStateChangedSignal.OnNext(_isBonusActive);
                    }
                }

                await UniTask.Delay(TimeSpan.FromSeconds(_decreaseActiveBonusGaugeDelay), cancellationToken: token);
            }
        }

        private async UniTask DecreaseBonusGaugeAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_bonusGauge > 0)
                {
                    _bonusGauge--;
                    float currentBonus = _bonusGauge / _maxBonusGauge;
                    _bonusGaugeChangedSignal.OnNext(currentBonus);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(_decreaseBonusGaugeDelay), ignoreTimeScale: true, cancellationToken: token);
            }
        }
    }
}