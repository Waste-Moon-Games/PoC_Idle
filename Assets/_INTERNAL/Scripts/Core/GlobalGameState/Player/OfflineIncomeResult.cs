namespace Core.GlobalGameState.Player
{
    public readonly struct OfflineIncomeResult
    {
        public long OfflineSeconds { get; }
        public float Income { get; }

        public OfflineIncomeResult(long offlineSeconds, float income)
        {
            OfflineSeconds = offlineSeconds;
            Income = income;
        }
    }
}