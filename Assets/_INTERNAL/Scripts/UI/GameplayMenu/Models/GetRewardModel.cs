using Common.MVVM;
using Core.LevelingSystem;
using R3;

namespace UI.GameplayMenu.Models
{
    public class GetRewardModel: IModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly Subject<RewardState> _rewardStateSignal = new();

        private RewardState _state;
        private readonly int _rewardId;
        private readonly RewardsSystemModel _model;

        public Observable<RewardState> RequestedRewardStateSignal => _rewardStateSignal.AsObservable();

        public GetRewardModel(RewardsSystemModel model, RewardByLevelData source)
        {
            _rewardId = source.RequiredLevel;
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
        }

        public void SubscribeOnUnlockSignal(Observable<RewardByLevelData> unlockSignal)
        {
            unlockSignal.Subscribe(source => 
            {
                _rewardStateSignal.OnNext(source.State);
            }).AddTo(_disposables);
        }

        public void SubscribeOnRecieveSignal(Observable<RewardByLevelData> recieveSignal)
        {
            recieveSignal.Subscribe(source => 
            {
                _rewardStateSignal.OnNext(source.State);
            }).AddTo(_disposables);
        }

        public void TryToReciveThisReward()
        {
            if (_state == RewardState.Recieved)
                return;

            _model.TryToReciveReward(_rewardId);
        }
    }
}