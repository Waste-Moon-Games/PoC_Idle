using R3;

namespace Core.Common.Player
{
    public interface ICurrencyWallet
    {
        float Amount { get; }
        Observable<float> AmountChanged { get; }

        bool TryToSpend(float amount);
        void Add(float amount);
        void ForceAdd(float amount);
    }
}