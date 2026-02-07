using Common.MVVM;
using Core.LevelingSystem;
using R3;
using UnityEngine;

namespace UI.GameplayMenu.Models
{
    public class RewardModel: IModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly Subject<RewardState> _rewardStateSignal = new();
        private readonly BehaviorSubject<float> _rewardAmountSignal;

        private RewardState _state;
        private readonly int _rewardId;
        private readonly float _rewardAmount;
        private readonly RewardsSystemModel _model;

        public Observable<RewardState> RequestedRewardStateSignal => _rewardStateSignal.AsObservable();
        public Observable<float> RewardAmountSignal => _rewardAmountSignal.AsObservable();

        public RewardModel(RewardsSystemModel model, RewardByLevelData source)
        {
            _rewardAmountSignal = new(0f);

            _rewardId = source.RequiredLevel;
            _rewardAmount = source.RewardAmount;

            _rewardAmountSignal.OnNext(_rewardAmount);

            _model = model;

            _state = source.State;
        }

        public void Dispose() => _disposables.Dispose();

        public void RequestRewardState() => _model.RequestRewardState(_rewardId);

        public void SubscribeOnRequestRewardStateSignal(Observable<RewardState> requestRewardStateSignal)
        {
            requestRewardStateSignal.Subscribe(source => 
            {
                _state = source;
                _rewardStateSignal.OnNext(_state);
            }).AddTo(_disposables);

            Debug.Log($"Reward ID: {_rewardId}, state: {_state}");
        }

        public void SubscribeOnUnlockSignal(Observable<RewardByLevelData> unlockSignal)
        {
            unlockSignal.Subscribe(source => 
            {
                _rewardStateSignal.OnNext(source.State);
            }).AddTo(_disposables);

            Debug.Log($"Reward ID: {_rewardId}, state: {_state}");
        }

        public void SubscribeOnRecieveSignal(Observable<RewardByLevelData> recieveSignal)
        {
            recieveSignal.Subscribe(source => 
            {
                _rewardStateSignal.OnNext(source.State);
            }).AddTo(_disposables);

            Debug.Log($"Reward ID: {_rewardId}, state: {_state}");
        }

        public void TryToReciveThisReward()
        {
            if (_state == RewardState.Recieved)
                return;

            _model.TryToReciveReward(_rewardId);
        }
    }
}