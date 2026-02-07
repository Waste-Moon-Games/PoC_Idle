using UnityEngine;
using Common.MVVM;
using UI.GameplayMenu.ViewModels;
using R3;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UI.GameplayMenu.Views
{
    public class RewardsSystemView : MonoBehaviour, IView
    {
        [Header("Content settings")]
        [SerializeField] private RewardView _rewardViewPrefab;
        [SerializeField] private Transform _rewardViewsContainer;

        [Space(10), Header("Other")]
        [SerializeField] private Button _openRewardsPanelButton;
        [SerializeField] private Button _closeRewardsPanelButton;
        [SerializeField] private GameObject _rewardsPanel;

        private readonly CompositeDisposable _disposables = new();

        private readonly List<RewardView> _rewadViews = new();

        private RewardsSystemViewModel _viewModel;

        private void Start()
        {
            if(_openRewardsPanelButton == null || _closeRewardsPanelButton == null)
                return;

            _openRewardsPanelButton.onClick.AddListener(OpenRewardsPanel);
            _closeRewardsPanelButton.onClick.AddListener(CloseRewardsPanel);
        }

        private void OnDestroy()
        {
            if(_openRewardsPanelButton == null || _closeRewardsPanelButton == null)
                return;

            _openRewardsPanelButton.onClick.RemoveListener(OpenRewardsPanel);
            _closeRewardsPanelButton.onClick.RemoveListener(CloseRewardsPanel);

            _disposables.Dispose();
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as RewardsSystemViewModel;

            _viewModel.RequestedRewardViewModels.Subscribe(HandleRequestedRewardModels).AddTo(_disposables);

            _viewModel.RequestedRewardModels();
        }

        private void OpenRewardsPanel()
        {
            if(!_rewardsPanel.activeSelf)
                _rewardsPanel.SetActive(true);
        }

        private void CloseRewardsPanel()
        {
            if(_rewardsPanel.activeSelf)
                _rewardsPanel.SetActive(false);
        }

        private void HandleRequestedRewardModels(List<RewardViewModel> rewardViewModels)
        {
            foreach(var viewModel in rewardViewModels)
            {
                var reward = Instantiate(_rewardViewPrefab, _rewardViewsContainer);
                reward.BindViewModel(viewModel);
                _rewadViews.Add(reward);
            }
        }
    }
}