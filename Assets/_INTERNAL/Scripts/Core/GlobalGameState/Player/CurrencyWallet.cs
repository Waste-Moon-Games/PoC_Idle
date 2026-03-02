using Core.Common.Player;
using R3;
using System;

namespace Core.GlobalGameState.Player
{
    public class CurrencyWallet : ICurrencyWallet, IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly string _walletName;

        private float _amount;

        private readonly BehaviorSubject<float> _amountChangedSignal;

        public float Amount
        {
            get { return _amount; }
            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(_amount), "Amount cannot be a negative!");

                _amount = value;
                _amountChangedSignal.OnNext(_amount);
            }
        }

        public Observable<float> AmountChanged => _amountChangedSignal.AsObservable();

        public CurrencyWallet(string walletName, float initialAmount)
        {
            _walletName = walletName;
            _amount = initialAmount;

            _amountChangedSignal = new(_amount);
        }

        public void Add(float amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            Amount += amount;
        }

        public void Dispose() => _disposables.Dispose();

        public void ForceAdd(float amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            Amount += amount;
        }

        public bool TryToSpend(float amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (Amount < amount)
                return false;

            Amount -= amount;
            return true;
        }
    }
}
