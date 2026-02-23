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

        private readonly Subject<RewardByLevel> _requestRewardStateSignal = new();
        private readonly Subject<RewardByLevel> _rewardUnlockSignal = new();
        private readonly Subject<BaseReward> _rewardReceiveSignal = new();

        private readonly RewardsByLevelConfig _rewardsByLevelConfig;
        private readonly CyclicRewardsConfig _cyclicRewardsConfig;
        private readonly PlayerEconomyService _economyService;
        private readonly Dictionary<int, RewardByLevel> _rewardsDict;
        private readonly Dictionary<int, CyclicReward> _cyclicRewardsDict;

        private readonly int _maxRewardsLevel;
        private readonly int _cyclicRewardLevelIncreaseStep;
        private readonly float _cyclicRewardAmountIncreaseStep;

        public IReadOnlyDictionary<int, RewardByLevel> RewardsDict => _rewardsDict;
        public Observable<RewardByLevel> RewardUnlocked => _rewardUnlockSignal.AsObservable();
        public Observable<BaseReward> RewardReceived => _rewardReceiveSignal.AsObservable();
        public Observable<RewardByLevel> RequestedRewardState => _requestRewardStateSignal.AsObservable();

        public PlayerRewardsByLevelService(RewardsByLevelConfig rewardsByLevelConfig, CyclicRewardsConfig cyclicRewardsConfig, Observable<int> levelChangedSignal, PlayerEconomyService economyService)
        {
            _rewardsByLevelConfig = rewardsByLevelConfig;
            _cyclicRewardsConfig = cyclicRewardsConfig;

            _economyService = economyService;

            _rewardsDict = _rewardsByLevelConfig.RewardsByLevel.ToDictionary(r => r.RewardRequiredLevel, r => r);
            _cyclicRewardsDict = _cyclicRewardsConfig.CyclicRewards.ToDictionary(c => c.RewardRequiredLevel, c => c);

            _maxRewardsLevel = _rewardsDict.Keys.Max();

            _cyclicRewardLevelIncreaseStep = _cyclicRewardsConfig.CyclicRewardRequiredLevelIncreaseStep;
            _cyclicRewardAmountIncreaseStep = _cyclicRewardsConfig.CyclicRewardAmountIncreaseStep;

            levelChangedSignal.Subscribe(HandleChangedLevel).AddTo(_disposables);
        }

        private void TryToAutoReceiveRewardByLevel(int level)
        {
            if(TryReceiveCyclicReward(level, out var reward))
            {
                if (reward.RewardType == RewardType.ClickBonus)
                    _economyService.IncreasePlayerClickByLevel(reward.RewardAmount);
                else
                    _economyService.IncreasePlayerPassiveIncomeByLevel(reward.RewardAmount);

                _rewardReceiveSignal.OnNext(reward);
                CascadeCyclicReward(reward);
            }
        }

        private bool TryReceiveCyclicReward(int level, out CyclicReward reward)
        {
            reward = null;

            if (!_cyclicRewardsDict.TryGetValue(level, out var currentReward))
                return false;

            if (!currentReward.TryToReceive())
                return false;

            reward = currentReward;
            return true;
        }

        private void CascadeCyclicReward(CyclicReward receivedReward)
        {
            int currentLevel = receivedReward.RewardRequiredLevel;

            int nextLevel = currentLevel + _cyclicRewardLevelIncreaseStep;

            float nextAmount = receivedReward.RewardAmount + _cyclicRewardAmountIncreaseStep;

            RewardType type = receivedReward.RewardType;

            _cyclicRewardsDict.Remove(currentLevel);

            var newReward = new CyclicReward(nextAmount, nextLevel, type);
            _cyclicRewardsDict[nextLevel] = newReward;
        }

        private bool TryToUnlockRewardByLevel(int level)
        {
            var reward = _rewardsDict.TryGetValue(level, out var result) ? result : null;
            if(reward is not null && reward.CanBeUnlocked())
            {
                reward.TryToUnlock();
                _rewardUnlockSignal.OnNext(reward);
                return true;
            }
            
            return false;
        }        

        private void HandleChangedLevel(int currentLevel)
        {
            if (currentLevel <= _maxRewardsLevel)
                TryToUnlockRewardByLevel(currentLevel);
            
            if(currentLevel % 5 == 0 && currentLevel > _maxRewardsLevel)
                TryToAutoReceiveRewardByLevel(currentLevel);
        }
        
        public void RequestRewardState(int rewardKey)
        {
            if(_rewardsDict.TryGetValue(rewardKey, out var reward))
                _requestRewardStateSignal.OnNext(reward);
        }

        public bool TryToReciveReward(int rewardKey)
        {
            var reward = _rewardsDict.TryGetValue(rewardKey, out var result) ? result : null;
            if(reward is not null && reward.CanBeReceived())
            {
                reward.TryToReceive();
                _rewardReceiveSignal.OnNext(reward);
                _economyService.AddRewardByLevel(reward.RewardAmount);
                return true;
            }

            return false;
        }

        public void Dispose() => _disposables.Dispose();
    }
}