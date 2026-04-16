using System.Collections.Generic;
using Common.MVVM;
using R3;
using UI.GameplayMenu.Models;

namespace UI.GameplayMenu.ViewModels
{
    public class RewardsSystemViewModel : IViewModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly Subject<List<RewardViewModel>> _requestRewardViewModelsSignal = new();
        private readonly BehaviorSubject<bool> _hasAvailableRewardsSignal;

        private readonly List<RewardViewModel> _rewardViewModels = new();

        private RewardsSystemModel _model;

        public Observable<List<RewardViewModel>> RequestedRewardViewModels => _requestRewardViewModelsSignal.AsObservable();
        public Observable<bool> HasAvailableRewardsSignal => _hasAvailableRewardsSignal.AsObservable();

        public RewardsSystemViewModel() => _hasAvailableRewardsSignal = new(false);

        public void BindModel(IModel model)
        {
            _model = model as RewardsSystemModel;

            _model.RequestedRewardModels.Subscribe(HandleRequestedRewardModels).AddTo(_disposables);
            _model.HasAvailableRewardsSignal.Subscribe(HandleAvailableRewardsSignal).AddTo(_disposables);
        }

        public void RequestedRewardModels() => _model.RequestRewardModels();

        public void Dispose()
        {
            _model.Dispose();
            _disposables.Dispose();
        }

        private void HandleAvailableRewardsSignal(bool value) => _hasAvailableRewardsSignal.OnNext(value);

        private void HandleRequestedRewardModels(List<RewardModel> models)
        {
            foreach(var model in models)
            {
                var viewModel = new RewardViewModel();
                viewModel.BindModel(model);
                _rewardViewModels.Add(viewModel);
            }

            _requestRewardViewModelsSignal.OnNext(_rewardViewModels);
        }
    }
}