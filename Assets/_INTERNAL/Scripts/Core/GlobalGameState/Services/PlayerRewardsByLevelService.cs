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

        private readonly Subject<RewardState> _requestRewardStateSignal = new();
        private readonly Subject<RewardByLevelData> _rewardUnlockSignal = new();
        private readonly Subject<RewardByLevelData> _rewardReciveSignal = new();

        private readonly RewardsByLevelConfig _config;
        private readonly PlayerEconomyService _economyService;
        private readonly Dictionary<int, RewardByLevelData> _rewardsDict;

        private readonly int _maxRewardsLevel;

        public IReadOnlyDictionary<int, RewardByLevelData> RewardsDict => _rewardsDict;
        public Observable<RewardByLevelData> RewardUnlocked => _rewardUnlockSignal.AsObservable();
        public Observable<RewardByLevelData> RewardRecieved => _rewardReciveSignal.AsObservable();
        public Observable<RewardState> RequestedRewardState => _requestRewardStateSignal.AsObservable();

        public PlayerRewardsByLevelService(RewardsByLevelConfig config, Observable<int> levelChangedSignal, PlayerEconomyService economyService)
        {
            _config = config;
            _economyService = economyService;

            _rewardsDict = _config.RewardsByLevel.ToDictionary(r => r.RequiredLevel, r => r);
            _maxRewardsLevel = _rewardsDict.Keys.Max();
            levelChangedSignal.Subscribe(HandleChangedLevel).AddTo(_disposables);
        }

        private bool TryToUnlockRewardByLevel(int level)
        {
            var reward = _rewardsDict.TryGetValue(level, out var result) ? result : null;
            if(reward != null && reward.CanBeUnlocked())
            {
                reward.MarkAsUnlocked();
                _rewardUnlockSignal.OnNext(reward);
                return true;
            }

            return false;
        }        

        private void HandleChangedLevel(int currentLevel)
        {
            if (currentLevel <= _maxRewardsLevel)
                TryToUnlockRewardByLevel(currentLevel);
        }
        
        public void RequestRewardState(int rewardKey)
        {
            if(_rewardsDict.TryGetValue(rewardKey, out var reward))
                _requestRewardStateSignal.OnNext(reward.State);
        }

        public bool TryToReciveReward(int rewardKey)
        {
            var reward = _rewardsDict.TryGetValue(rewardKey, out var result) ? result : null;
            if(reward != null && reward.CanBeRecieved())
            {
                reward.MarkAsRecived();
                _rewardReciveSignal.OnNext(reward);
                _economyService.AddReward(reward.RewardAmount);
                return true;
            }

            return false;
        }

        public void Dispose() => _disposables.Dispose();
    }
}