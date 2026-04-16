using System.Collections.Generic;
using System.Linq;
using Common.MVVM;
using Core.GlobalGameState.Services;
using R3;

namespace UI.GameplayMenu.Models
{
    public class RewardsSystemModel : IModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly Subject<List<RewardModel>> _requestRewardModelsSignal = new();
        private readonly BehaviorSubject<bool> _hasAvailableRewardsSignal;

        private readonly List<RewardModel> _rewardModels = new();

        private readonly PlayerRewardsByLevelService _rewardsService;

        public Observable<List<RewardModel>> RequestedRewardModels => _requestRewardModelsSignal.AsObservable();
        public Observable<bool> HasAvailableRewardsSignal => _hasAvailableRewardsSignal.AsObservable();

        public RewardsSystemModel(PlayerRewardsByLevelService rewardsService)
        {
            _rewardsService = rewardsService;
            _hasAvailableRewardsSignal = new(false);

            _rewardsService.HasAvailableRewardsSignal.Subscribe(HandleAvailableRewardsSignal).AddTo(_disposables);
        }

        public void InitRewards()
        {
            var rewardsList = _rewardsService.RewardsDict;
            foreach(var rewardItem in rewardsList.Values)
                _rewardModels.Add(new RewardModel(this, rewardItem));

            InitRewardModels();
            FindLastReward().SetItLast(true);
        }

        public void RequestRewardModels() => _requestRewardModelsSignal.OnNext(_rewardModels);

        public void RequestRewardState(int keyId) => _rewardsService.RequestRewardState(keyId);
        public void TryToReceiveReward(int keyId) => _rewardsService.TryToReciveReward(keyId);

        public void Dispose() => _disposables.Dispose();

        private void InitRewardModels()
        {
            foreach(var model in _rewardModels)
            {
                model.SubscribeOnUnlockSignal(_rewardsService.RewardUnlocked);
                model.SubscribeOnReceiveSignal(_rewardsService.RewardReceived);
                model.SubscribeOnRequestRewardStateSignal(_rewardsService.RequestedRewardState);
            }
        }

        private RewardModel FindLastReward()
        {
            RewardModel result = _rewardModels.Last();
            return result;
        }

        private void HandleAvailableRewardsSignal(bool value) => _hasAvailableRewardsSignal.OnNext(value);
    }
}