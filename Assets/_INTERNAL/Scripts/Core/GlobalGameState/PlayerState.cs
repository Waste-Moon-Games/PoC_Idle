using Core.GlobalGameState.Services;
using Cysharp.Threading.Tasks;
using R3;
using SO.PlayerConfigs;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using Utils.ModCoroutines;

namespace Core.GlobalGameState
{
    public class PlayerState
    {
        private CancellationTokenSource _cts;

        private readonly Subject<float> _bonusGaugeChangedSignal = new();
        private readonly Subject<int> _levelChangedSignal = new();
        private readonly Subject<int> _currentExpChangedSignal = new();
        private readonly Subject<int> _expToLevelUpChangedSignal = new();

        private readonly PlayerEconomyService _playerEconomyService;
        private readonly PlayerUpgradeService _playerUpgradeService;
        private readonly ShopState _shopState;

        private readonly Coroutines _coroutines;

        private float _bonusGauge;
        private float _bonusPerClick;
        private readonly float _decreaseBonusGaugeDelay;

        private readonly float _expIncreaseMultiplier;
        private int _level;
        private int _expPerClick;
        private int _currentExp;
        private int _expToLevelUp;

        public PlayerEconomyService EconomyService => _playerEconomyService;
        public PlayerUpgradeService UpgradeService => _playerUpgradeService;
        public ShopState ShopState => _shopState;

        public Observable<float> BonusGaugeChanged => _bonusGaugeChangedSignal.AsObservable();
        public Observable<int> LevelChanged => _levelChangedSignal.AsObservable();
        public Observable<int> CurrentExpChanged => _currentExpChangedSignal.AsObservable();
        public Observable<int> ExpToLevelUpChanged => _expToLevelUpChangedSignal.AsObservable();

        public PlayerState()
        {
            var economyConfig = Resources.Load<MainEconomyConfig>("Configs/Economy/MainEconomyConfig");

            _playerEconomyService = new(economyConfig);
            _playerUpgradeService = new(_playerEconomyService);
            _shopState = new();

            var playerConfig = Resources.Load<PlayerConfig>("Configs/Player/PlayerConfig");
            _bonusGauge = playerConfig.PlayerBonusGauge;
            _bonusPerClick = playerConfig.InitPlayerBonusPerClick;

            _level = 1;
            _currentExp = 0;
            _expToLevelUp = playerConfig.InitExpToLevelUp;
            _expPerClick = playerConfig.InitPlayerExpPerClick;
            _expIncreaseMultiplier = playerConfig.ExpIncreaseMultiplier;
            _decreaseBonusGaugeDelay = playerConfig.DecreaseBonusGaugeDelay;
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
            _playerEconomyService.StartAsyncTasks();
        }

        public void StopAsyncTasks()
        {
            _playerEconomyService.StopAllTasks();
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