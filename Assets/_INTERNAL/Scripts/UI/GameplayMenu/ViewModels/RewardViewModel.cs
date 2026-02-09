using Common.MVVM;
using Core.LevelingSystem;
using R3;
using UI.GameplayMenu.Models;

namespace UI.GameplayMenu.ViewModels
{
    public class RewardViewModel: IViewModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly Subject<RewardState> _requestedRewardStateSignal = new();

        private RewardState _state;

        private RewardModel _model;

        public Observable<RewardState> RequestedRewardState => _requestedRewardStateSignal.AsObservable();
        public Observable<float> RewardAmountSignal => _model.RewardAmountSignal.AsObservable();

        private void HandleRequestedRewardState(RewardState state)
        {
            _state = state;
            _requestedRewardStateSignal.OnNext(state);
        }

        public void BindModel(IModel model)
        {
            _model = model as RewardModel;

            _model.RequestedRewardStateSignal.Subscribe(HandleRequestedRewardState).AddTo(_disposables);
        }

        public void Dispose()
        {
            _model?.Dispose();
            _disposables.Dispose();
        }

        public void RequestRewardState() => _model.RequestRewardState();
        public void TryToReciveThisReward() => _model.TryToReceiveThisReward();
    }
}