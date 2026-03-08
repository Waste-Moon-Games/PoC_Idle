using Core.Common.Player;
using Core.GlobalGameState.Player;
using Core.SaveSystemBase.Data;
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

        #region Subjects
        private readonly BehaviorSubject<float> _playerClickAmountChangedSignal;
        private readonly Subject<float> _coinsClickAdSignal = new();
        private readonly BehaviorSubject<float> _passiveIncomeAmountChangedSignal;
        #endregion

        #region PlayerWallets
        private readonly ICurrencyWallet _coinsWallet;
        private readonly ICurrencyWallet _gemsWallet;
        #endregion

        #region Delays
        private readonly float _passiveIncomeDelay;
        #endregion

        #region Gems Reward Chance
        private int _gemsClickRewardAmount = 1;
        private float _gemsRewardClickChance = 0f;
        private readonly float _minGemsRewardClickChance;
        private readonly float _maxGemsRewardClickChance;
        #endregion

        #region Tripple Click Chance
        private float _trippleClickChance;
        private readonly float _minTrippleClickChance;
        private readonly float _maxTrippleClickChance;
        #endregion

        #region Bonuses
        private float _playerClickAmount;
        private float _playerClickBonusAmount = 1f;
        private float _passiveIncomeAmount;
        private float _passiveIncomeBonusAmount = 1f;
        private bool _bonusState;
        #endregion

        #region Multipliers
        private readonly float _bonusClickMultiplier;
        private readonly float _defaultClickMultiplier = 1f;
        #endregion

        #region Properties
        public float PlayerClickAmount
        {
            get => _playerClickAmount;
            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(_playerClickAmount), "AddCoins price cannot be a negative");

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
        public ICurrencyWallet CoinsWallet => _coinsWallet;
        public ICurrencyWallet GemsWallet => _gemsWallet;
        #endregion

        #region Wallets Observables
        public Observable<float> CoinsWalletChanged => _coinsWallet.AmountChanged.AsObservable();
        public Observable<float> GemsWalletChanged => _gemsWallet.AmountChanged.AsObservable();
        #endregion

        #region UI Observables
        public Observable<float> PlayerClickAmountChanged => _playerClickAmountChangedSignal.AsObservable();
        public Observable<float> CoinsClickAd => _coinsClickAdSignal.AsObservable();
        public Observable<float> PassiveIncomeChanged => _passiveIncomeAmountChangedSignal.AsObservable();
        #endregion

        public PlayerEconomyService(MainEconomyConfig config, Observable<bool> bonusStateChaged, float bonusClickMultiplier)
        {
            _coinsWallet = new CurrencyWallet("Coins_Wallet", config.InitialCoinsWalletAmount);
            _gemsWallet = new CurrencyWallet("Gems_Wallet", config.InitialGemsWalletAmount);

            _gemsRewardClickChance = config.InitialGemsClickRewardChance;
            _minGemsRewardClickChance = config.InitialGemsClickRewardChance;
            _maxGemsRewardClickChance = config.MaxGemsClickRewardChance;
            _gemsClickRewardAmount = config.InitialGemsClickRewardAmount;

            _playerClickAmountChangedSignal = new(0f);
            _passiveIncomeAmountChangedSignal = new(0f);

            PlayerClickAmount = config.InitialPlayerClickAmount;

            PassiveIncomeAmount = 0f;
            _passiveIncomeDelay = config.PassiveIncomeDelay;

            TrippleClickChance = config.InitialCurrentTrippleClickChance;

            _minTrippleClickChance = config.InitialMinTrippleClickChance;
            _maxTrippleClickChance = config.MaxTrippleClickChance;

            bonusStateChaged.Subscribe(HandleChangedBonusState).AddTo(_disposables);
            _bonusClickMultiplier = bonusClickMultiplier;

            _passiveIncomeAmountChangedSignal.AddTo(_disposables);
        }

        public PlayerEconomyService(
            MainEconomyConfig config,
            Observable<bool> bonusStateChaged,
            float bonusClickMultiplier,
            PlayerData loadedData)
        {
            _coinsWallet = new CurrencyWallet("Coins_Wallet", config.InitialCoinsWalletAmount);
            _gemsWallet = new CurrencyWallet("Gems_Wallet", config.InitialGemsWalletAmount);

            _gemsRewardClickChance = config.InitialGemsClickRewardChance;
            _minGemsRewardClickChance = config.InitialGemsClickRewardChance;
            _maxGemsRewardClickChance = config.MaxGemsClickRewardChance;
            _gemsClickRewardAmount = config.InitialGemsClickRewardAmount;

            _playerClickAmountChangedSignal = new(0f);
            _passiveIncomeAmountChangedSignal = new(0f);

            PlayerClickAmount = loadedData.PlayerClickAmount;

            PassiveIncomeAmount = loadedData.PassiveIncomeAmount;
            _passiveIncomeDelay = config.PassiveIncomeDelay;

            TrippleClickChance = loadedData.TrippleClickChance;

            _minTrippleClickChance = config.InitialMinTrippleClickChance;
            _maxTrippleClickChance = config.MaxTrippleClickChance;

            bonusStateChaged.Subscribe(HandleChangedBonusState).AddTo(_disposables);
            _bonusClickMultiplier = bonusClickMultiplier;

            _passiveIncomeAmountChangedSignal.AddTo(_disposables);
        }

        public void IncreasePlayerClick(float amount) => PlayerClickAmount += amount;

        public void IncreaseTrippleClickChance(float amount) => TrippleClickChance = MathF.Min(TrippleClickChance + amount, _maxTrippleClickChance);

        public void IncreasePlayerPassiveIncome(float amount) => PassiveIncomeAmount += amount;

        public void IncreasePlayerClickByLevel(float amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            _playerClickBonusAmount += amount;

            PlayerClickAmount *= _playerClickBonusAmount;
        }

        public void IncreasePlayerPassiveIncomeByLevel(float amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            _passiveIncomeBonusAmount += amount;

            PassiveIncomeAmount *= _passiveIncomeBonusAmount;
        }

        public void AddCoinsRewardByLevel(float amount) => _coinsWallet.Add(amount);
        public void AddGemsRewardByLevel(float amount) => _gemsWallet.Add(amount);

        public void AddCoins()
        {
            float clickReward = CalculateReward();

            _coinsWallet.Add(clickReward);
            if (TryAddGems())
            {
                int clickGemsReward = CalculateGemsReward();
                _gemsWallet.Add(clickGemsReward);
            }

            _coinsClickAdSignal.OnNext(clickReward);
        }

        public bool TryToSpend(float amount) => _coinsWallet.TryToSpend(amount);

        public bool HasEnoughCoins(float amount) => amount >= 0 && _coinsWallet.Amount >= amount;

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

        public void Dispose()
        {
            _disposables.Dispose();
            (_coinsWallet as IDisposable).Dispose();
            (_gemsWallet as IDisposable).Dispose();
        }

        private void HandleChangedBonusState(bool state) => _bonusState = state;

        private bool TryAddGems()
        {
            float randRoll = RandRoll(_minGemsRewardClickChance, _maxGemsRewardClickChance);

            return randRoll < _gemsRewardClickChance;
        }

        private int CalculateGemsReward()
        {
            //TODO: сделать рандомный бросок на x2 получение гемов за клик
            int result = _gemsClickRewardAmount;

            return result;
        }

        private float CalculateReward()
        {
            float randRoll = RandRoll(_minTrippleClickChance, _maxTrippleClickChance);
            float rewardMultiplier = (randRoll < TrippleClickChance) ? _bonusClickMultiplier : _defaultClickMultiplier;

            float localClick = PlayerClickAmount;
            float clickReward;

            if (!_bonusState)
                clickReward = localClick * rewardMultiplier;
            else
                clickReward = localClick * rewardMultiplier * _bonusClickMultiplier;

            return clickReward;
        }

        private float RandRoll(float minValue, float maxValue)
        {
            float randRollResult = Random.Range(minValue, maxValue);

            return randRollResult;
        }

        private float CalculatePassiveIncomeTick()
        {
            return PassiveIncomeAmount;
        }

        private void ApplyPassiveIncome()
        {
            var income = CalculatePassiveIncomeTick();
            if (income <= 0f)
                return;

            _coinsWallet.Add(income);
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