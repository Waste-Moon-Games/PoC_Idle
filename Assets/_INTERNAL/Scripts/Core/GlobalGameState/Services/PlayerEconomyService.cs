using Cysharp.Threading.Tasks;
using R3;
using SO.PlayerConfigs;
using System;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.GlobalGameState.Services
{
    public class PlayerEconomyService
    {
        private CancellationTokenSource _cts;

        private readonly Subject<float> _coinsChangeSignal = new();
        private readonly Subject<float> _coinsPerClickChangeSignal = new();
        private readonly Subject<float> _coinsPerClickSignal = new();
        private readonly Subject<float> _passiveIncomeAmountChangeSignal = new();

        private float _playerWallet;
        private readonly float _passiveIncomeDelay;

        private float _trippleClickChance;
        private readonly float _minTrippleClickChance;
        private readonly float _maxTrippleClickChance;

        private float _playerClickAmount;
        private float _passiveIncomeAmount;

        public float PlayerWallet
        {
            get => _playerWallet;
            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(_playerWallet), "A wallet cannot be negative");
                _playerWallet = value;
            }
        }
        public float PlayerClickAmount
        {
            get => _playerClickAmount;
            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(_playerClickAmount), "Add price cannot be a negative");
                _playerClickAmount = value;
            }
        }
        public float PassiveIncomeAmount
        {
            get => _passiveIncomeAmount;
            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(_passiveIncomeAmount), "Value cannot be a negative! (Passive Income)");
                _passiveIncomeAmount = value;
            }
        }
        public float TrippleClickChance
        {
            get => _trippleClickChance;
            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(_trippleClickChance), "Value cannot be a negative! (Tripple Click Chance)");
                _trippleClickChance = value;
            }
        }

        public Observable<float> CoinsChanged => _coinsChangeSignal.AsObservable();
        public Observable<float> CoinsPerClickChanged => _coinsPerClickChangeSignal.AsObservable();
        public Observable<float> CoinsPerClick => _coinsPerClickSignal.AsObservable();
        public Observable<float> PassiveInconeChanged => _passiveIncomeAmountChangeSignal.AsObservable();

        public PlayerEconomyService(MainEconomyConfig config)
        {
            _playerClickAmount = config.InitialPlayerClickAmount;
            _playerWallet = config.InitialPlayerWallet;

            _passiveIncomeAmount = 0f;
            _passiveIncomeDelay = config.PassiveIncomeDelay;

            _trippleClickChance = config.InitialCurrentTrippleClickChance;

            _minTrippleClickChance = config.InitialMinTrippleClickChance;
            _maxTrippleClickChance = config.MaxTrippleClickChance;
        }

        public void IncreasePlayerClick(float amount)
        {
            PlayerClickAmount += amount;
            _coinsPerClickChangeSignal.OnNext(PlayerClickAmount);
        }

        public void IncreaseTrippleClickChance(float amount)
        {
            if(_trippleClickChance < _maxTrippleClickChance)
            {
                _trippleClickChance += amount;
                return;
            }

            _trippleClickChance = _maxTrippleClickChance;
        }

        public void IncreasePlayerPassiveIncome(float amount)
        {
            PassiveIncomeAmount += amount;
            _passiveIncomeAmountChangeSignal.OnNext(PassiveIncomeAmount);
        }

        public void RequestCurrentPassiveIncome() => _passiveIncomeAmountChangeSignal.OnNext(PassiveIncomeAmount);

        public void Add()
        {
            float randRoll = Random.Range(_minTrippleClickChance, _maxTrippleClickChance);
            float rewardMultiplier = (randRoll < _trippleClickChance) ? 3f : 1f;

            float localClick = PlayerClickAmount;
            float clickReward = localClick * rewardMultiplier;
            PlayerWallet += clickReward;

            _coinsChangeSignal.OnNext(PlayerWallet);
            _coinsPerClickSignal.OnNext(clickReward);
        }

        public void Spend(float amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be a negative!");

            PlayerWallet -= amount;
            _coinsChangeSignal.OnNext(PlayerWallet);
        }

        public bool HasEnoughCoins(float amount)
        {
            if (PlayerWallet >= amount)
                return true;

            return false;
        }

        public void StartAsyncTasks()
        {
            _cts = new CancellationTokenSource();
            PassiveIncomeAsync(_cts.Token).Forget();
        }

        public void StopAllTasks()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            _coinsChangeSignal.OnCompleted();
            _coinsPerClickChangeSignal.OnCompleted();
            _coinsPerClickSignal.OnCompleted();
            _passiveIncomeAmountChangeSignal.OnCompleted();
        }

        private void ApplyPassiveIncome()
        {
            PlayerWallet += PassiveIncomeAmount;
            _coinsChangeSignal.OnNext(PlayerWallet);
        }

        private async UniTask PassiveIncomeAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                ApplyPassiveIncome();
                await UniTask.Delay(TimeSpan.FromSeconds(_passiveIncomeDelay), ignoreTimeScale: true, cancellationToken: token);
            }
        }
    }
}