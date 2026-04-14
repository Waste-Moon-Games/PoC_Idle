using Core.Common.Player;
using Core.GlobalGameState.Player;
using Core.SaveSystemBase.Data;

using Cysharp.Threading.Tasks;
using R3;

using SO.PlayerConfigs;

using System;
using System.Collections.Generic;
using System.Threading;

using Random = UnityEngine.Random;

namespace Core.GlobalGameState.Services
{
    public class PlayerEconomyService
    {
        private CancellationTokenSource _cts;
        private readonly CompositeDisposable _disposables = new();

        private readonly BehaviorSubject<float> _playerClickAmountChangedSignal;
        private readonly BehaviorSubject<float> _passiveIncomeAmountChangedSignal;
        private readonly Subject<float> _coinsClickAdSignal = new();

        private readonly Dictionary<CurrencyType, ICurrencyWallet> _wallets = new();

        private readonly float _passiveIncomeDelay;

        private int _gemsClickRewardAmount = 1;
        private float _gemsRewardClickChance = 0f;
        private readonly float _minGemsRewardClickChance;
        private readonly float _maxGemsRewardClickChance;

        private float _trippleClickChance;
        private readonly float _minTripleClickChance;
        private readonly float _maxTripleClickChance;

        private float _playerClickAmount;
        private float _playerClickBonusAmount = 1f;
        private float _passiveIncomeAmount;
        private float _passiveIncomeBonusAmount = 1f;

        private bool _bonusState;
        private bool _rewardedBonusState;

        private readonly float _bonusClickMultiplier;
        private readonly float _defaultClickMultiplier = 1f;
        private readonly float _rewardedBonusClickMultiplier;

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

        public float TripleClickChance
        {
            get => _trippleClickChance;
            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(_trippleClickChance), "Value cannot be a negative! (Tripple Click Chance)");

                _trippleClickChance = value;
            }
        }

        public float GemsClickRewardChance
        {
            get => _gemsRewardClickChance;
            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(_gemsRewardClickChance));

                _gemsRewardClickChance = value;
            }
        }

        public ICurrencyWallet CoinsWallet => _wallets[CurrencyType.Coins];
        public ICurrencyWallet GemsWallet => _wallets[CurrencyType.Gems];

        public Observable<float> CoinsWalletChanged => CoinsWallet.AmountChanged.AsObservable();
        public Observable<float> GemsWalletChanged => GemsWallet.AmountChanged.AsObservable();

        public Observable<float> PlayerClickAmountChanged => _playerClickAmountChangedSignal.AsObservable();
        public Observable<float> CoinsClickAd => _coinsClickAdSignal.AsObservable();
        public Observable<float> PassiveIncomeChanged => _passiveIncomeAmountChangedSignal.AsObservable();

        public PlayerEconomyService(
            MainEconomyConfig config,
            Observable<bool> bonusStateChaged,
            PlayerRewardedBonusesService playerRewardedBonusesService,
            float bonusClickMultiplier)
            : this(config, bonusStateChaged, playerRewardedBonusesService, bonusClickMultiplier, null)
        {
        }

        public PlayerEconomyService(
            MainEconomyConfig config,
            Observable<bool> bonusStateChaged,
            PlayerRewardedBonusesService playerRewardedBonusesService,
            float bonusClickMultiplier,
            PlayerData loadedData)
        {
            _wallets[CurrencyType.Coins] = new CurrencyWallet("Coins_Wallet", loadedData?.Coins ?? config.InitialCoinsWalletAmount);
            _wallets[CurrencyType.Gems] = new CurrencyWallet("Gems_Wallet", loadedData?.Gems ?? config.InitialGemsWalletAmount);

            GemsClickRewardChance = config.InitialGemsClickRewardChance;
            _minGemsRewardClickChance = config.InitialMinGemsClickRewardChance;
            _maxGemsRewardClickChance = config.MaxGemsClickRewardChance;
            _gemsClickRewardAmount = config.InitialGemsClickRewardAmount;

            _playerClickAmountChangedSignal = new(0f);
            _passiveIncomeAmountChangedSignal = new(0f);

            PlayerClickAmount = loadedData?.PlayerClickAmount ?? config.InitialPlayerClickAmount;
            PassiveIncomeAmount = loadedData?.PassiveIncomeAmount ?? 0f;
            _passiveIncomeDelay = config.PassiveIncomeDelay;

            TripleClickChance = loadedData?.TrippleClickChance ?? config.InitialCurrentTrippleClickChance;
            _minTripleClickChance = config.InitialMinTrippleClickChance;
            _maxTripleClickChance = config.MaxTrippleClickChance;

            bonusStateChaged.Subscribe(HandleChangedBonusState).AddTo(_disposables);

            playerRewardedBonusesService.TemporaryBonusStateChanged.Subscribe(HandleChangedStateRewardBonus).AddTo(_disposables);
            playerRewardedBonusesService.CurrencyBonusGiven.Subscribe(HandleGivenCurrencyBonus).AddTo(_disposables);

            _bonusClickMultiplier = bonusClickMultiplier;
            _rewardedBonusClickMultiplier = bonusClickMultiplier;

            _passiveIncomeAmountChangedSignal.AddTo(_disposables);
        }

        public void IncreasePlayerClick(float amount) => PlayerClickAmount += amount;
        public void IncreaseTripleClickChance(float amount) => TripleClickChance = MathF.Min(TripleClickChance + amount, _maxTripleClickChance);
        public void IncreaseGemsRewardClickChance(float amount) => GemsClickRewardChance = MathF.Min(GemsClickRewardChance + amount, _maxGemsRewardClickChance);
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

        public void AddCoinsByOtherIcnome(float amount) => CoinsWallet.Add(amount);
        public void AddGemsRewardByLevel(float amount) => GemsWallet.Add(amount);

        public void AddCoins()
        {
            float clickReward = CalculateReward();

            CoinsWallet.Add(clickReward);

            if (TryAddGems())
            {
                int clickGemsReward = CalculateGemsReward();
                GemsWallet.Add(clickGemsReward);
            }

            _coinsClickAdSignal.OnNext(clickReward);
        }

        public bool TryToSpend(CurrencyType currencyType, float amount)
        {
            if (amount < 0)
                return false;

            return _wallets.TryGetValue(currencyType, out var wallet) && wallet.TryToSpend(amount);
        }

        public bool HasEnough(CurrencyType currencyType, float amount)
            => amount >= 0 && _wallets.TryGetValue(currencyType, out var wallet) && wallet.Amount >= amount;

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
            foreach (var wallet in _wallets.Values)
                (wallet as IDisposable)?.Dispose();
        }

        private bool TryAddGems()
        {
            float randRoll = RandRoll(_minGemsRewardClickChance, _maxGemsRewardClickChance);
            return randRoll < GemsClickRewardChance;
        }

        private int CalculateGemsReward() => _gemsClickRewardAmount;

        private float CalculateReward()
        {
            float randRoll = RandRoll(_minTripleClickChance, _maxTripleClickChance);
            float rewardMultiplier = randRoll < TripleClickChance ? _bonusClickMultiplier : _defaultClickMultiplier;

            if (_bonusState)
                rewardMultiplier *= _bonusClickMultiplier;
            if (_rewardedBonusState)
                rewardMultiplier *= _rewardedBonusClickMultiplier;

            float result = PlayerClickAmount * rewardMultiplier;

            return result;
        }

        private float RandRoll(float minValue, float maxValue) => Random.Range(minValue, maxValue);

        private float CalculatePassiveIncomeTick()
        {
            float result = PassiveIncomeAmount;

            if(_rewardedBonusState)
                result = PassiveIncomeAmount * _bonusClickMultiplier;

            return result;
        }

        private void ApplyPassiveIncome()
        {
            var income = CalculatePassiveIncomeTick();
            if (income <= 0f)
                return;

            CoinsWallet.Add(income);
        }

        private async UniTask PassiveIncomeAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                ApplyPassiveIncome();
                await UniTask.Delay(TimeSpan.FromSeconds(_passiveIncomeDelay), ignoreTimeScale: true, cancellationToken: token);
            }
        }

        private void HandleChangedBonusState(bool state) => _bonusState = state;
        private void HandleChangedStateRewardBonus(bool state) => _rewardedBonusState = state;
        private void HandleGivenCurrencyBonus(float amount) => GemsWallet.Add(amount);
    }
}