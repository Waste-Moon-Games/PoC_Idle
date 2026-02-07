using Common.MVVM;
using Core.Common.Animations;
using R3;
using TMPro;
using UI.GameplayMenu.Animations;
using UI.GameplayMenu.ViewModels;
using UnityEngine;
using Utils.Formatter;

namespace UI.GameplayMenu.Views
{
    public class EconomyPlayerInfoView : MonoBehaviour, IView
    {
        private readonly CompositeDisposable _disposables = new();

        [Header("Player State Info")]
        [SerializeField] private TextMeshProUGUI _currentCoinsCount;
        [SerializeField] private TextMeshProUGUI _currentCoinsPerClick;
        [SerializeField] private TextMeshProUGUI _currentPassiveIncome;

        private EconomyPlayerInfoViewModel _viewModel;
        private NumberFormatter _formatter;
        private AnimationService _animationsService;

        private void OnDestroy()
        {
            _viewModel.Dispose();
            _disposables.Dispose();
        }

        public void BindFormatter(NumberFormatter formatter) => _formatter = formatter;

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as EconomyPlayerInfoViewModel;

            _viewModel.CoinsChanged.Subscribe(UpdateCoinsCount).AddTo(_disposables);
            _viewModel.CoinsPerClickChanged.Subscribe(UpdateCoinsPerClick).AddTo(_disposables);
            _viewModel.CoinsClickAd.Subscribe(UpdateAnimations).AddTo(_disposables);
            _viewModel.PassiveIncomeChanged.Subscribe(UpdateCurrentPassiveIncome).AddTo(_disposables);
        }

        public void BindAnimationService(ClickAnimationsService animationService) => _animationsService = animationService;

        private void UpdateCoinsCount(float amount) => _currentCoinsCount.text = $"<color=yellow>{_formatter.FormatNumber(amount)}</color>";

        private void UpdateCoinsPerClick(float amount) => _currentCoinsPerClick.text = $"<color=#00FFFF>{_formatter.FormatNumber(amount)}</color>/Click";

        private void UpdateAnimations(float amount) => _animationsService?.ClickAnimation(_formatter.FormatNumber(amount));

        private void UpdateCurrentPassiveIncome(float amount) => _currentPassiveIncome.text = $"<color=green>{_formatter.FormatNumber(amount)}</color>/Sec";
    }
}