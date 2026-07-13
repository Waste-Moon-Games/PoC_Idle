using Common.MVVM;

using Core.Common.Animations;

using DG.Tweening;

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

        [Space(5), Header("Player GemsPerClick Info")]
        [SerializeField] private TextMeshProUGUI _currentGemsCount;

        [Space(5), Header("Player Temporary Bonus Info")]
        [SerializeField] private TemporaryBonusView _temporaryBonus;

        [Space(5), Header("Animations setup")]
        [SerializeField] private float _coinsAnimationDuration = 0.25f;
        [SerializeField] private float _gemsAnimationDuration = 0.25f;

        private int _displayedCoinsCount = 0;
        private int _displayedGemsCount = 0;
        private int _displayedCoinsPerClick = 0;
        private int _displayedPassiveIncome = 0;

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

            if (_temporaryBonus != null)
                _temporaryBonus.SetStartTime(_viewModel.InitialTemporaryBonusDurationInSeconds);

            _viewModel.CoinsChanged.Subscribe(UpdateCoinsCount).AddTo(_disposables);
            _viewModel.GemsChanged.Subscribe(UpdateGemsCount).AddTo(_disposables);

            _viewModel.CoinsClickAd.Subscribe(UpdateAnimations).AddTo(_disposables);

            _viewModel.CoinsPerClickChanged.Subscribe(UpdateCoinsPerClick).AddTo(_disposables);
            _viewModel.PassiveIncomeChanged.Subscribe(UpdateCurrentPassiveIncome).AddTo(_disposables);

            _viewModel.TemporaryBonusStateChanged.Subscribe(HandleChangedTemporaryBonusState).AddTo(_disposables);
            _viewModel.TemporartyBonusTimerChanged.Subscribe(UpdateTemporaryBonusTimer).AddTo(_disposables);
        }

        public void BindAnimationService(ClickAnimationsService animationService) => _animationsService = animationService;

        private void UpdateCoinsCount(float amount)
        {
            int oldAmount = _displayedCoinsCount;
            _displayedCoinsCount = Mathf.RoundToInt(amount);

            DOVirtual.Float(oldAmount, _displayedCoinsCount, _coinsAnimationDuration, value =>
            {
                _currentCoinsCount.text = $"<color=#FFCB7A>{_formatter.FormatNumber(Mathf.RoundToInt(value))}</color>";
            }).SetEase(Ease.OutQuad);
        }

        private void UpdateGemsCount(float amount)
        {
            int oldAmount = _displayedGemsCount;
            _displayedGemsCount = Mathf.RoundToInt(amount);

            DOVirtual.Float(oldAmount, _displayedGemsCount, _gemsAnimationDuration, value =>
            {
                _currentGemsCount.text = $"<color=#E78DFF>{_formatter.FormatNumber(Mathf.RoundToInt(value))}</color>";
            }).SetEase(Ease.OutQuad);
        }

        private void UpdateCoinsPerClick(float amount)
        {
            int oldAmount = _displayedCoinsPerClick;
            _displayedCoinsPerClick = Mathf.RoundToInt(amount);

            DOVirtual.Float(oldAmount, _displayedCoinsPerClick, _coinsAnimationDuration, value =>
            {
                _currentCoinsPerClick.text = $"<color=#00FFFF>{_formatter.FormatNumber(Mathf.RoundToInt(value))}</color>/Click";
            }).SetEase(Ease.OutQuad);
        }

        private void UpdateAnimations(float amount) => _animationsService?.ClickAnimation(_formatter.FormatNumber(amount));

        private void UpdateCurrentPassiveIncome(float amount)
        {
            int oldAmount = _displayedPassiveIncome;
            _displayedPassiveIncome = Mathf.RoundToInt(amount);

            DOVirtual.Float(oldAmount, _displayedPassiveIncome, _coinsAnimationDuration, value =>
            {
                _currentPassiveIncome.text = $"<color=green>{_formatter.FormatNumber(Mathf.RoundToInt(value))}</color>/Sec";
            }).SetEase(Ease.OutQuad);
        }

        private void UpdateTemporaryBonusTimer(float time) => _temporaryBonus.UpdateProgress(time);
        private void HandleChangedTemporaryBonusState(bool state) => _temporaryBonus.Toggle(state);
    }
}