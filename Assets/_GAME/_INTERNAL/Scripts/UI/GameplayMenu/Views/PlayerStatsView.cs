using Common.MVVM;
using DG.Tweening;
using R3;
using TMPro;
using UI.GameplayMenu.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameplayMenu.Views
{
    public class PlayerStatsView : MonoBehaviour, IView
    {
        private readonly CompositeDisposable _disposables = new();

        [SerializeField] private Image _expProgressBar;

        [Space(5), Header("Exp setup")]
        [SerializeField] private TextMeshProUGUI _currentLevelText;
        [SerializeField] private TextMeshProUGUI _expText;

        [Space(5), Header("Animation setup")]
        [SerializeField] private float _expChangeAnimationDuration = 0.5f;

        private int _displayedExp = 0;
        private PlayerStatsViewModel _viewModel;

        private void OnDestroy()
        {
            _viewModel.Dispose();
            _disposables.Dispose();
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as PlayerStatsViewModel;

            _viewModel.LevelChanged.Subscribe(HandleChangedLevel).AddTo(_disposables);
            _viewModel.ExpChanges.Subscribe(changes => HandleExpChanges(changes.Item1, changes.Item2)).AddTo(_disposables);

            _viewModel.RequestDefaultLevelState();
        }

        private void HandleChangedLevel(int level) => _currentLevelText.text = $"{level}";
        private void HandleExpChanges(int currentExp, int expToLevelUp)
        {
            float progress = (float)currentExp / (float)expToLevelUp;

            _expProgressBar.DOFillAmount(progress, _expChangeAnimationDuration).SetEase(Ease.OutQuad);

            int oldExp = _displayedExp;
            _displayedExp = currentExp;

            DOVirtual.Float(oldExp, _displayedExp, _expChangeAnimationDuration, value =>
            {
                _expText.text = $"{Mathf.RoundToInt(value)}/{expToLevelUp}";
            }).SetEase(Ease.OutQuad);
        }
    }
}