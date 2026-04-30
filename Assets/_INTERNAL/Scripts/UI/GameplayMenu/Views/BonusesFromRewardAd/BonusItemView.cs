using Common.MVVM;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using TMPro;
using UI.GameplayMenu.ViewModels.BonusesFromRewardAd;
using UnityEngine;
using UnityEngine.UI;
using Utils.Localization;

namespace UI.GameplayMenu.Views.BonusesFromRewardAd
{
    [RequireComponent(typeof(Button))]
    public class BonusItemView : MonoBehaviour, IView
    {
        [SerializeField] private Vector2 _activatedPosition;
        [SerializeField] private float _activateAnimationDuration = 0.65f;

        private readonly CompositeDisposable _disposables = new();

        private BonusItemViewModel _viewModel;

        private Button _openButton;

        private RectTransform _rectTransform;
        private Vector2 _defaultPosition;

        private void Awake()
        {
            _openButton = GetComponent<Button>();
            _rectTransform = GetComponent<RectTransform>();

            _openButton.onClick.AddListener(HandleClickedButton);

            _defaultPosition = _rectTransform.anchoredPosition;
        }

        private void OnDestroy()
        {
            _openButton.onClick.RemoveListener(HandleClickedButton);

            _disposables.Dispose();

            _rectTransform.DOKill();
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as BonusItemViewModel;

            if (_viewModel.IsActive)
            {
                _openButton.interactable = false;
                _rectTransform.anchoredPosition = _activatedPosition;
            }

            _viewModel.BonusInfoWindowStateChanged.Subscribe(_ =>
            {
                HandleBonusInfoWindowClosed();
            }).AddTo(_disposables);

            _viewModel.BonusActiveStateChanged.Subscribe(HandleBonusChangedState).AddTo(_disposables);
        }

        private void HandleBonusChangedState(bool state)
        {
            if (state)
            {
                _openButton.interactable = false;

                _rectTransform.DOAnchorPos(_activatedPosition, _activateAnimationDuration)
                    .SetEase(Ease.OutElastic)
                    .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            }
            else
            {
                gameObject.SetActive(true);
                _openButton.interactable = true;
                _rectTransform.DOAnchorPos(_defaultPosition, _activateAnimationDuration).SetEase(Ease.OutElastic);
            }
        }

        private void HandleBonusInfoWindowClosed() => _openButton.interactable = true;

        private void HandleClickedButton()
        {
            _viewModel.OpenItemWindow();
            _openButton.interactable = false;
        }
    }
}