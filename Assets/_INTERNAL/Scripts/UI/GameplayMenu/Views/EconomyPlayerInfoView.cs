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

        [Header("Player Coins Info")]
        [SerializeField] private TextMeshProUGUI _currentCoinsCount;
        [SerializeField] private TextMeshProUGUI _currentCoinsPerClick;
        [SerializeField] private TextMeshProUGUI _currentPassiveIncome;

        [Space(5), Header("Player Gems Info")]
        [SerializeField] private TextMeshProUGUI _currentGemsCount;

        [Space(5), Header("Player Temporary Bonus Info")]
        [SerializeField] private TemporaryBonusView _temporaryBonus;

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
            _viewModel.GemsChanged.Subscribe(UpdateGemsCount).AddTo(_disposables);

            _viewModel.CoinsClickAd.Subscribe(UpdateAnimations).AddTo(_disposables);

            _viewModel.CoinsPerClickChanged.Subscribe(UpdateCoinsPerClick).AddTo(_disposables);
            _viewModel.PassiveIncomeChanged.Subscribe(UpdateCurrentPassiveIncome).AddTo(_disposables);

            _viewModel.TemporartyBonusTimerChanged.Take(1).Subscribe(HandleInitTemporaryBonusTimer).AddTo(_disposables);
            _viewModel.TemporaryBonusStateChanged.Subscribe(HandleChangedTemporaryBonusState).AddTo(_disposables);
            _viewModel.TemporartyBonusTimerChanged.Subscribe(UpdateTemporaryBonusTimer).AddTo(_disposables);
        }

        public void BindAnimationService(ClickAnimationsService animationService) => _animationsService = animationService;

        private void UpdateCoinsCount(float amount) => _currentCoinsCount.text = $"<color=yellow>{_formatter.FormatNumber(amount)}</color>";
        private void UpdateGemsCount(float amount) => _currentGemsCount.text = $"<color=red>{_formatter.FormatNumber(amount)}</color>";

        private void UpdateCoinsPerClick(float amount) => _currentCoinsPerClick.text = $"<color=#00FFFF>{_formatter.FormatNumber(amount)}</color>/Click";

        private void UpdateAnimations(float amount) => _animationsService?.ClickAnimation(_formatter.FormatNumber(amount));

        private void UpdateCurrentPassiveIncome(float amount) => _currentPassiveIncome.text = $"<color=green>{_formatter.FormatNumber(amount)}</color>/Sec";

        private void UpdateTemporaryBonusTimer(float time) => _temporaryBonus.UpdateProgress(time);
        private void HandleInitTemporaryBonusTimer(float time) => _temporaryBonus.SetStartTime(time);
        private void HandleChangedTemporaryBonusState(bool state) => _temporaryBonus.Toggle(state);
    }
}