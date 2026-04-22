using UnityEngine;
using Common.MVVM;
using UI.GameplayMenu.ViewModels;
using R3;
using System.Collections.Generic;
using UnityEngine.UI;
using Core.GlobalGameState;
using Core.AudioSystemCommon;

namespace UI.GameplayMenu.Views
{
    public class RewardsSystemView : MonoBehaviour, IView
    {
        [Header("Content settings")]
        [SerializeField] private RewardView _rewardViewPrefab;
        [SerializeField] private Transform _rewardViewsContainer;

        [Space(10), Header("Other")]
        [SerializeField] private Button _openRewardsPanelButton;
        [SerializeField] private Image _chestIcon;
        [SerializeField] private Button _closeRewardsPanelButton;
        [SerializeField] private GameObject _rewardsPanel;
        [SerializeField] private Sprite _availableRewardsChestSprite;
        [SerializeField] private Sprite _defaultChestSprite;

        private readonly CompositeDisposable _disposables = new();

        private readonly List<RewardView> _rewadViews = new();

        private RewardsSystemViewModel _viewModel;
        private AudioSystemService _audioSystemService;

        private void Start()
        {
            if(_openRewardsPanelButton == null || _closeRewardsPanelButton == null)
                return;

            transform.SetAsLastSibling();
            _openRewardsPanelButton.onClick.AddListener(OpenRewardsPanel);
            _closeRewardsPanelButton.onClick.AddListener(CloseRewardsPanel);
        }

        private void OnDestroy()
        {
            if(_openRewardsPanelButton == null || _closeRewardsPanelButton == null)
                return;

            _openRewardsPanelButton.onClick.RemoveListener(OpenRewardsPanel);
            _closeRewardsPanelButton.onClick.RemoveListener(CloseRewardsPanel);

            _viewModel.Dispose();
            _disposables.Dispose();
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as RewardsSystemViewModel;

            _viewModel.RequestedRewardViewModels.Subscribe(HandleRequestedRewardModels).AddTo(_disposables);
            _viewModel.HasAvailableRewardsSignal.Subscribe(HandleAvailableRewardsSignal).AddTo(_disposables);

            _viewModel.RequestedRewardModels();
        }

        public void BindAudioSystemService(AudioSystemService audioSystemService)
        {
            _audioSystemService = audioSystemService;
        }

        private void OpenRewardsPanel()
        {
            if(!_rewardsPanel.activeSelf)
                _rewardsPanel.SetActive(true);

            _audioSystemService.PlaySoundByID(SoundsIds.CloseSound);
        }

        private void CloseRewardsPanel()
        {
            if(_rewardsPanel.activeSelf)
                _rewardsPanel.SetActive(false);

            _audioSystemService.PlaySoundByID(SoundsIds.CloseSound);
        }

        private void HandleAvailableRewardsSignal(bool value)
        {
            if (value)
                _chestIcon.sprite = _availableRewardsChestSprite;
            else
                _chestIcon.sprite = _defaultChestSprite;
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