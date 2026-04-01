using Common.MVVM;
using Cysharp.Threading.Tasks;
using R3;
using UI.GameplayMenu.ViewModels.BonusesFromRewardAd;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameplayMenu.Views.BonusesFromRewardAd
{
    [RequireComponent(typeof(Button))]
    public class BonusItemView : MonoBehaviour, IView
    {
        private readonly CompositeDisposable _disposables = new();

        private BonusItemViewModel _viewModel;

        private Button _openButton;

        private void Awake()
        {
            _openButton = GetComponent<Button>();

            _openButton.onClick.AddListener(HandleClickedButton);
        }

        private void OnDestroy()
        {
            _openButton.onClick.RemoveListener(HandleClickedButton);

            _disposables.Dispose();
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as BonusItemViewModel;

            _viewModel.BonusInfoWindowStateChanged.Subscribe(_ =>
            {
                HandleBonusInfoWindowClosed();
            }).AddTo(_disposables);
        }

        private void HandleBonusInfoWindowClosed() => _openButton.interactable = true;

        private void HandleClickedButton()
        {
            _viewModel.OpenItemWindow();
            _openButton.interactable = false;
        }
    }
}