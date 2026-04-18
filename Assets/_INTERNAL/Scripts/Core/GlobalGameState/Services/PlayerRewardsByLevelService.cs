using Core.Consts;
using Core.LevelingSystem;
using Core.SaveSystemBase.Data;
using R3;
using SO.PlayerConfigs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.GlobalGameState.Services
{
    public class PlayerRewardsByLevelService
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly Subject<RewardByLevelRuntime> _requestRewardStateSignal = new();
        private readonly Subject<RewardByLevelRuntime> _rewardUnlockSignal = new();
        private readonly Subject<BaseReward> _rewardReceiveSignal = new();
        private readonly BehaviorSubject<bool> _hasAvailableRewardsSignal;

        private readonly RewardsByLevelConfig _rewardsByLevelConfig;
        private readonly CyclicRewardsConfig _cyclicRewardsConfig;
        private readonly PlayerEconomyService _economyService;
        private readonly Dictionary<int, RewardByLevelRuntime> _rewardsDict;
        private readonly Dictionary<int, CyclicRewardRuntime> _cyclicRewardsDict;

        private readonly int _maxRewardsLevel;
        private readonly int _cyclicRewardLevelIncreaseStep;
        private readonly float _cyclicRewardAmountIncreaseStep;

        public IReadOnlyDictionary<int, RewardByLevelRuntime> RewardsDict => _rewardsDict;
        public IReadOnlyDictionary<int, CyclicRewardRuntime> CyclicRewardsDict => _cyclicRewardsDict;

        public Observable<RewardByLevelRuntime> RewardUnlocked => _rewardUnlockSignal.AsObservable();
        public Observable<BaseReward> RewardReceived => _rewardReceiveSignal.AsObservable();
        public Observable<RewardByLevelRuntime> RequestedRewardState => _requestRewardStateSignal.AsObservable();
        public Observable<bool> HasAvailableRewardsSignal => _hasAvailableRewardsSignal.AsObservable();

        public PlayerRewardsByLevelService(
            RewardsByLevelConfig rewardsByLevelConfig,
            CyclicRewardsConfig cyclicRewardsConfig,
            Observable<int> levelChangedSignal,
            PlayerEconomyService economyService) : 
            this(rewardsByLevelConfig, cyclicRewardsConfig, levelChangedSignal, economyService, null)
        { }

        public PlayerRewardsByLevelService(
            RewardsByLevelConfig rewardsByLevelConfig,
            CyclicRewardsConfig cyclicRewardsConfig,
            Observable<int> levelChangedSignal,
            PlayerEconomyService economyService,
            PlayerData loadedData)
        {
            _rewardsByLevelConfig = rewardsByLevelConfig;
            _cyclicRewardsConfig = cyclicRewardsConfig;

            _economyService = economyService;

            _rewardsDict = BuildRewardsByLevelDictionary(loadedData);
            _cyclicRewardsDict = BuildCyclicRewardsDictionary(loadedData);

            _maxRewardsLevel = _rewardsDict.Keys.Max();

            _cyclicRewardLevelIncreaseStep = _cyclicRewardsConfig.CyclicRewardRequiredLevelIncreaseStep;
            _cyclicRewardAmountIncreaseStep = _cyclicRewardsConfig.CyclicRewardAmountIncreaseStep;

            levelChangedSignal.Subscribe(HandleChangedLevel).AddTo(_disposables);

            _hasAvailableRewardsSignal = new(false);
        }

        private Dictionary<int, RewardByLevelRuntime> BuildRewardsByLevelDictionary(PlayerData loadedData)
        {
            var result = RewardFactory
                .CreateRewardsByLevelList(_rewardsByLevelConfig.RewardsByLevel)
                .ToDictionary(r => r.RewardRequiredLevel, r => r);

            if (loadedData?.ReceivedRewards is not { Count: > 0 })
                return result;

            foreach (var savedData in loadedData.ReceivedRewards)
            {
                int requiredLevel = TryResolveRewardLevel(savedData);
                if (requiredLevel <= 0 || !result.TryGetValue(requiredLevel, out var runtimeReward))
                    continue;

                if(ResolveRewardState(savedData) == RewardState.Received)
                {
                    runtimeReward.TryToUnlock();
                    runtimeReward.TryToReceive();
                }
            }

            return result;
        }

        private Dictionary<int, CyclicRewardRuntime> BuildCyclicRewardsDictionary(PlayerData loadedData)
        {
            var defaultDictionary = RewardFactory
                .CreateCyclicRewardsList(_cyclicRewardsConfig.CyclicRewards)
                .ToDictionary(c => c.RewardRequiredLevel, c => c);

            if (loadedData?.CyclicRewards is not { Count: > 0 })
                return defaultDictionary;

            var preparedRewards = new List<RewardData>(loadedData.CyclicRewards.Count);
            foreach (var reward in loadedData.CyclicRewards)
            {
                int requiredLevel = TryResolveRewardLevel(reward);
                if (requiredLevel <= 0)
                    continue;

                preparedRewards.Add(new RewardData
                {
                    ID = reward.ID,
                    RequiredLevel = requiredLevel,
                    RewardAmount = reward.RewardAmount,
                    Type = reward.Type,
                    State = reward.State,
                    Received = reward.Received
                });
            }

            if (preparedRewards.Count == 0)
                return defaultDictionary;

            return RewardFactory
                .CreateCyclicRewardsList(preparedRewards)
                .GroupBy(c => c.RewardRequiredLevel)
                .ToDictionary(g => g.Key, g => g.Last());
        }

        private int TryResolveRewardLevel(RewardData rewardData)
        {
            if (rewardData == null)
                return 0;

            if(rewardData.RequiredLevel > 0)
                return rewardData.RequiredLevel;

            if (string.IsNullOrWhiteSpace(rewardData.ID))
                return 0;

            return 0;
        }

        private RewardState ResolveRewardState(RewardData rewardData)
        {
            if (rewardData == null)
                return RewardState.Locked;

            if(rewardData.State == RewardState.Received)
                return RewardState.Received;

            return Enum.IsDefined(typeof(RewardState), rewardData.State)
                ? rewardData.State
                : RewardState.Locked;
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

        private bool TryReceiveCyclicReward(int level, out CyclicRewardRuntime reward)
        {
            reward = null;

            if (!_cyclicRewardsDict.TryGetValue(level, out var currentReward))
                return false;

            if (!currentReward.TryToReceive())
                return false;

            reward = currentReward;
            return true;
        }

        private void CascadeCyclicReward(CyclicRewardRuntime receivedReward)
        {
            int currentLevel = receivedReward.RewardRequiredLevel;

            int nextLevel = currentLevel + _cyclicRewardLevelIncreaseStep;

            float nextAmount = receivedReward.RewardAmount + _cyclicRewardAmountIncreaseStep;

            RewardType type = receivedReward.RewardType;

            _cyclicRewardsDict.Remove(currentLevel);

            var newReward = new CyclicRewardRuntime(nextAmount, nextLevel, type);
            _cyclicRewardsDict[nextLevel] = newReward;
        }

        private bool TryToUnlockRewardByLevel(int level)
        {
            var reward = _rewardsDict.TryGetValue(level, out var result) ? result : null;
            if(reward is not null && reward.CanBeUnlocked())
            {
                reward.TryToUnlock();
                _rewardUnlockSignal.OnNext(reward);
                _hasAvailableRewardsSignal.OnNext(true);
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
                _hasAvailableRewardsSignal.OnNext(false);

                switch (reward.RewardType)
                {
                    case RewardType.Coins:
                        _economyService.AddCoinsByOtherIcnome(reward.RewardAmount);
                        break;
                    case RewardType.Gems:
                        _economyService.AddGemsRewardByLevel(reward.RewardAmount);
                        break;
                }
                return true;
            }

            return false;
        }

        public void Dispose() => _disposables.Dispose();
    }
}