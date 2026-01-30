using Common.MVVM;
using R3;
using UI.GameplayMenu.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameplayMenu.Views
{
    public class BonusGaugeZoneView : MonoBehaviour, IView
    {
        private readonly CompositeDisposable _disposables = new();

        [SerializeField] private Image _bonusGaugeProgressBar;
        [SerializeField] private GameObject _progressBar;

        private MainGameViewModel _viewModel;

        private void OnDestroy()
        {
            _disposables.Dispose();
            _viewModel?.Dispose();
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as MainGameViewModel;

            _viewModel.ChangedBonusGauge.Subscribe(HandleChangedBonusGauge).AddTo(_disposables);
            _viewModel.RequestDefaultBonusGaugeState();
        }

        private void HandleChangedBonusGauge(float amount)
        {
            if (amount <= 0)
                _progressBar.SetActive(false);
            else
                _progressBar.SetActive(true);

            _bonusGaugeProgressBar.fillAmount = amount;
        }
    }
}