using Common.MVVM;
using R3;
using System.Collections.Generic;
using System.Linq;
using UI.GameplayMenu.ViewModels.BonusesFromRewardAd;
using UnityEngine;

namespace UI.GameplayMenu.Views.BonusesFromRewardAd
{
    public class PlayerRewardedBonusesView : MonoBehaviour, IView
    {
        private readonly CompositeDisposable _disposables = new();

        [SerializeField] private List<BonusItemView> _items = new();
        [SerializeField] private RewardFromAdBonusInfoView _bonusInfoView;

        private PlayerRewardedBonusesViewModel _viewModel;

        private void Start()
        {
            if (_items.Capacity == 0)
            {
                Debug.LogWarning("[Player Bonuses From Reward Ads View] Bonus Item list is empty!");
                return;
            }

            _bonusInfoView.ShowAdButtonClickSingal.Subscribe(_ =>
            {
                HandleShowAdButtonClick();
            }).AddTo(_disposables);

            _bonusInfoView.CloseButtonClickSignal.Subscribe(_ =>
            {
                HandleCloseButtonClick();
            }).AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
            _viewModel?.Dispose();
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as PlayerRewardedBonusesViewModel;

            _viewModel.BonusSelectedSignal.Subscribe(HandleSelectedBonusItem).AddTo(_disposables);
            for (int i = 0; i < _viewModel.BonusItemViewModels.Count; i++)
            {
                var itemViewModel = _viewModel.BonusItemViewModels[i];
                var itemView = _items[i];

                itemView.BindViewModel(itemViewModel);
            }
        }

        private void HandleSelectedBonusItem(string selectedItemDesc)
        {
            _bonusInfoView.SetDescription(selectedItemDesc);
            _bonusInfoView.Open();
        }

        private void HandleCloseButtonClick()
        {
            _viewModel.CloseBonusInfoWindow();
        }

        private void HandleShowAdButtonClick()
        {
            _viewModel.ShowAd();
        }
    }
}