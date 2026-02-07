using Cysharp.Threading.Tasks;
using R3;
using SO.PlayerConfigs;
using System;
using System.Threading;
using Random = UnityEngine.Random;

namespace Core.GlobalGameState.Services
{
    public class PlayerEconomyService
    {
        private CancellationTokenSource _cts;
        private readonly CompositeDisposable _disposables = new();

        private readonly BehaviorSubject<float> _playerWalletChagedSignal;
        private readonly BehaviorSubject<float> _playerClickAmountChangedSignal;
        private readonly Subject<float> _coinsClickAdSignal = new();
        private readonly BehaviorSubject<float> _passiveIncomeAmountChangedSignal;

        private float _playerWallet;
        private readonly float _passiveIncomeDelay;

        private float _trippleClickChance;
        private readonly float _minTrippleClickChance;
        private readonly float _maxTrippleClickChance;

        private float _playerClickAmount;
        private float _passiveIncomeAmount;
        private bool _bonusState;
        private readonly float _bonusClickMultiplier;
        private readonly float _defaultClickMultiplier = 1f;

        public float PlayerWallet
        {
            get => _playerWallet;
            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(_playerWallet), "A wallet cannot be negative");

                _playerWallet = value;
                _playerWalletChagedSignal.OnNext(_playerWallet);
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
                _playerClickAmountChangedSignal.OnNext(_playerClickAmount);
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
                _passiveIncomeAmountChangedSignal.OnNext(_passiveIncomeAmount);
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

        public Observable<float> PlayerWalletChanged => _playerWalletChagedSignal.AsObservable();
        public Observable<float> PlayerClickAmountChanged => _playerClickAmountChangedSignal.AsObservable();
        public Observable<float> CoinsClickAd => _coinsClickAdSignal.AsObservable();
        public Observable<float> PassiveInconeChanged => _passiveIncomeAmountChangedSignal.AsObservable();

        public PlayerEconomyService(MainEconomyConfig config, Observable<bool> bonusStateChaged, float bonusClickMultiplier)
        {
            _playerWalletChagedSignal = new(0f);
            _playerClickAmountChangedSignal = new(0f);
            _passiveIncomeAmountChangedSignal = new(0f);

            PlayerClickAmount = config.InitialPlayerClickAmount;
            PlayerWallet = config.InitialPlayerWallet;

            PassiveIncomeAmount = 0f;
            _passiveIncomeDelay = config.PassiveIncomeDelay;

            TrippleClickChance = config.InitialCurrentTrippleClickChance;

            _minTrippleClickChance = config.InitialMinTrippleClickChance;
            _maxTrippleClickChance = config.MaxTrippleClickChance;

            bonusStateChaged.Subscribe(HandleChangedBonusState).AddTo(_disposables);
            _bonusClickMultiplier = bonusClickMultiplier;
        }

        public void IncreasePlayerClick(float amount) => PlayerClickAmount += amount;

        public void IncreaseTrippleClickChance(float amount) => TrippleClickChance = MathF.Min(TrippleClickChance + amount, _maxTrippleClickChance);

        public void IncreasePlayerPassiveIncome(float amount) => PassiveIncomeAmount += amount;

        public void AddReward(float amount)
        {
            if(amount <= 0)
                return;

            PlayerWallet += amount;
        }

        public void Add()
        {
            float randRoll = Random.Range(_minTrippleClickChance, _maxTrippleClickChance);
            float rewardMultiplier = (randRoll < TrippleClickChance) ? _bonusClickMultiplier : _defaultClickMultiplier;

            float localClick = PlayerClickAmount;
            float clickReward;

            if (!_bonusState)
                clickReward = localClick * rewardMultiplier;
            else
                clickReward = localClick * rewardMultiplier * _bonusClickMultiplier;

            PlayerWallet += clickReward;

            // _playerWalletChagedSignal.OnNext(PlayerWallet);
            _coinsClickAdSignal.OnNext(clickReward);
        }

        public void Spend(float amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be a negative!");

            PlayerWallet -= amount;
            // _playerWalletChagedSignal.OnNext(PlayerWallet);
        }

        public bool HasEnoughCoins(float amount) => amount >= 0 && PlayerWallet >= amount;

        public void StartAsyncTasks()
        {
            _cts = new CancellationTokenSource();
            PassiveIncomeAsync(_cts.Token).Forget();
        }

        public void StopAsyncTasks()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        public void Dispose() => _disposables.Dispose();

        private void HandleChangedBonusState(bool state) => _bonusState = state;

        private float CalculatePassiveIncomeTick()
        {
            return PassiveIncomeAmount;
        }

        private void ApplyPassiveIncome()
        {
            var income = CalculatePassiveIncomeTick();
            if(income <= 0f)
                return;

            PlayerWallet += income;
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