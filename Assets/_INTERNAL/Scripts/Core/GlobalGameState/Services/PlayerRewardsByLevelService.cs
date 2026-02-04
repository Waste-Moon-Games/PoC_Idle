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
        private readonly Subject<float> _rewardReciveSignal = new();

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

        private void TryToUnlockRewardByLevel(int level)
        {
            var reward = _rewardsDict.TryGetValue(level, out var result) ? result : null;
            if (reward == null || !reward.CanBeUnlocked())
                return;

            reward.MarkAsUnlocked();
            _rewardUnlockSignal.OnNext(reward);
        }        

        private void HandleChangedLevel(int currentLevel)
        {
            if (currentLevel <= _maxRewardsLevel)
                TryToUnlockRewardByLevel(currentLevel);
        }

        public bool TryToReciveReward(int rewardKey)
        {
            if(_rewardsDict.TryGetValue(rewardKey, out var reward))
            {
                reward.MarkAsUnlocked();
                _rewardReciveSignal.OnNext(reward.RewardAmount);
                return true;
            }

            return false;
        }

        public void Dispose() => _disposables.Dispose();
    }
}