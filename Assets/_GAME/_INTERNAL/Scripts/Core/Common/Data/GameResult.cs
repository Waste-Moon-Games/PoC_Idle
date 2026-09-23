namespace Core.Common.Data
{
    public readonly struct GameResult
    {
        public readonly bool IsWin;
        public readonly float RewardCoins;

        public GameResult(bool isWin, float rewardCoins)
        {
            IsWin = isWin;
            RewardCoins = rewardCoins;
        }
    }
}