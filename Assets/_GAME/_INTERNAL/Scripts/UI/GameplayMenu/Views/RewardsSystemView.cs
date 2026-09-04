using UnityEngine;
using Common.MVVM;
using UI.GameplayMenu.ViewModels;
using R3;
using System.Collections.Generic;
using UnityEngine.UI;
using Core.AudioSystemCommon;
using Utils.Localization;
using TMPro;
using UI.Common.Components;
using UI.GameplayMenu.Views.Panels;

namespace UI.GameplayMenu.Views
{
    public class RewardsSystemView : MonoBehaviour, IView
    {
        [Header("Content settings")]
        [SerializeField] private RewardView _rewardViewPrefab;
        [SerializeField] private Transform _rewardViewsContainer;
        [SerializeField] private TextMeshProUGUI _nameText;

        [Space(10), Header("Other")]
        [SerializeField] private ActionButton _openRewardsPanelButton;
        [SerializeField] private Image _chestIcon;
        [SerializeField] private ActionButton _closeRewardsPanelButton;
        [SerializeField] private RewardsPanelView _rewardsPanel;
        [SerializeField] private Sprite _availableRewardsChestSprite;
        [SerializeField] private Sprite _defaultChestSprite;

        [Space(5), Header("Localization setup")]
        [SerializeField] private LocalizedText _nameLocalizations;

        private readonly SoundType _openSoundType = SoundType.Open;
        private readonly SoundType _closeSoundType = SoundType.Close;

        private readonly CompositeDisposable _disposables = new();

        private readonly List<RewardView> _rewadViews = new();

        private RewardsSystemViewModel _viewModel;

        private void Start()
        {
            if(_openRewardsPanelButton == null || _closeRewardsPanelButton == null)
                return;

            transform.SetAsLastSibling();
            _openRewardsPanelButton.OnButtonClick += HandleOpenRewardsButtonClick;
            _closeRewardsPanelButton.OnButtonClick += HandleCloseRewardsButtonClick;

            _nameText.text = _nameLocalizations.Get(Application.systemLanguage);

            if (_rewardsPanel.gameObject.activeSelf)
            {
                _rewardsPanel.gameObject.SetActive(false);
                _rewardsPanel.MoveDisappearAnimation();
            }
        }

        private void OnDestroy()
        {
            if(_openRewardsPanelButton == null || _closeRewardsPanelButton == null)
                return;

            _openRewardsPanelButton.OnButtonClick -= HandleOpenRewardsButtonClick;
            _closeRewardsPanelButton.OnButtonClick -= HandleCloseRewardsButtonClick;

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

        private void HandleOpenRewardsButtonClick()
        {
            if(!_rewardsPanel.gameObject.activeSelf)
                _rewardsPanel.MoveAppearAnimation();

            AudioEventBus.InvokeSoundSignalByType(_openSoundType);
        }

        private void HandleCloseRewardsButtonClick()
        {
            if(_rewardsPanel.gameObject.activeSelf)
                _rewardsPanel.MoveDisappearAnimation();

            AudioEventBus.InvokeSoundSignalByType(_closeSoundType);
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