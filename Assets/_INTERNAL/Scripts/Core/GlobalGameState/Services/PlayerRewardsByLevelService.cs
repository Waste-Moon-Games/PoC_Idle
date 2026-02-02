using Core.LevelingSystem;
using R3;
using SO.PlayerConfigs;
using System.Collections.Generic;
using System.Linq;

namespace Core.GlobalGameState.Services
{
    public class PlayerRewardsByLevelService
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly Subject<RewardByLevelData> _rewardUnlockSignal = new();

        private readonly RewardsByLevelConfig _config;
        private readonly Dictionary<int, RewardByLevelData> _rewardsDict;

        private readonly int _maxRewardsLevel;

        public Observable<RewardByLevelData> RewardUnlocked => _rewardUnlockSignal.AsObservable();

        public PlayerRewardsByLevelService(RewardsByLevelConfig config, Observable<int> levelChangedSignal)
        {
            _config = config;

            _rewardsDict = _config.RewardsByLevel.ToDictionary(r => r.RequiredLevel, r => r);
            _maxRewardsLevel = _rewardsDict.Keys.Max();
            levelChangedSignal.Subscribe(HandleChangedLevel).AddTo(_disposables);
        }

        private void TryToGetRewardByLevel(int level)
        {
            var reward = _rewardsDict.TryGetValue(level, out var result) ? result : null;
            if (reward == null)
                return;

            _rewardUnlockSignal.OnNext(reward);
        }

        private void HandleChangedLevel(int currentLevel)
        {
            if (currentLevel <= _maxRewardsLevel)
                TryToGetRewardByLevel(currentLevel);
        }

        public void Dispose() => _disposables.Dispose();
    }
}