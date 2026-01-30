using Common.MVVM;
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

        [Space(5), Header("Level/Exp Info")]
        [SerializeField] private TextMeshProUGUI _currentLevelText;
        [SerializeField] private TextMeshProUGUI _expText;

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
            _expText.text = $"{currentExp}/{expToLevelUp}";
            _expProgressBar.fillAmount = progress;
        }
    }
}