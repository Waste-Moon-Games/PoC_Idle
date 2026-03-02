using Common.MVVM;
using Core.LevelingSystem;
using R3;
using Utils.Extensions;

namespace UI.GameplayMenu.Models
{
    public class RewardModel: IModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly Subject<RewardState> _rewardStateSignal = new();
        private readonly BehaviorSubject<float> _rewardAmountSignal;
        private readonly BehaviorSubject<int> _rewardRequiredLevelSignal;
        private readonly BehaviorSubject<bool> _conntectorStateSignal;

        private RewardState _state;
        private readonly int _rewardId;
        private readonly float _rewardAmount;
        private readonly RewardsSystemModel _model;

        public Observable<RewardState> RequestedRewardStateSignal => _rewardStateSignal.AsObservable();
        public Observable<float> RewardAmountSignal => _rewardAmountSignal.AsObservable();
        public Observable<int> RewardRequiredLevelSignal => _rewardRequiredLevelSignal.AsObservable();
        public Observable<bool> ConnectorStateSignal => _conntectorStateSignal.AsObservable();

        public RewardModel(RewardsSystemModel model, RewardByLevelRuntime source)
        {
            _rewardAmountSignal = new(0f);
            _rewardRequiredLevelSignal = new(0);
            _conntectorStateSignal = new(false);

            _rewardId = source.RewardRequiredLevel;
            _rewardAmount = source.RewardAmount;

            _rewardAmountSignal.OnNext(_rewardAmount);
            _rewardRequiredLevelSignal.OnNext(_rewardId);

            _model = model;

            _state = source.RewardState;
        }

        public void Dispose() => _disposables.Dispose();

        public void RequestRewardState() => _model.RequestRewardState(_rewardId);
        public void SetItLast(bool state) => _conntectorStateSignal.OnNext(state);

        public void SubscribeOnRequestRewardStateSignal(Observable<RewardByLevelRuntime> requestRewardStateSignal)
        {
            requestRewardStateSignal
                .Where(source => source.RewardRequiredLevel == _rewardId)
                .Subscribe(source => 
                {
                    _state = source.RewardState;
                    _rewardStateSignal.OnNext(_state);
                }).AddTo(_disposables);
        }

        public void SubscribeOnUnlockSignal(Observable<RewardByLevelRuntime> unlockSignal)
        {
            unlockSignal
                .Where(source => source.RewardRequiredLevel == _rewardId)
                .Subscribe(source => 
                {
                    _rewardStateSignal.OnNext(source.RewardState);
                }).AddTo(_disposables);
        }

        public void SubscribeOnReceiveSignal(Observable<BaseReward> receiveSignal)
        {
            receiveSignal
                .Select(source => (object)source)
                .OfType<RewardByLevelRuntime>()
                .Where(reward => reward.RewardRequiredLevel == _rewardId)
                .Subscribe(reward => 
                {
                    _rewardStateSignal.OnNext(reward.RewardState);
                }).AddTo(_disposables);
        }

        public void TryToReceiveThisReward()
        {
            if (_state == RewardState.Received)
                return;

            _model.TryToReceiveReward(_rewardId);
        }
    }
}