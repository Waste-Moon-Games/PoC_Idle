using Common.MVVM;
using DG.Tweening;
using R3;
using TMPro;
using UI.GameplayMenu.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameplayMenu.Views
{
    public class OfflineIncomeView : MonoBehaviour, IView
    {
        private readonly CompositeDisposable _disposables = new();

        [Header("Text setup")]
        [SerializeField] private TextMeshProUGUI _offlineHours;
        [SerializeField] private TextMeshProUGUI _offlineIncome;

        [Space(5), Header("Buttons setup")]
        [SerializeField] private Button _receiveIncome;
        [SerializeField] private Button _receiveDoubleIncome;

        [Space(5), Header("Animation setup")]
        [SerializeField] private float _openCloseAnimDuration = 1f;
        [SerializeField] private Vector2 _receivedScale = new(0f, 0f);
        [SerializeField] private Vector2 _defaultScale = Vector2.one;

        private OfflineIncomeViewModel _viewModel;

        private bool _isCanBeOpened;

        private RectTransform _rectTransform;

        private void Awake() => _rectTransform = GetComponent<RectTransform>();

        private void Start()
        {
            

            if(_receiveIncome == null || _receiveDoubleIncome == null)
            {
                Debug.LogWarning("[Offline Income View] One or more buttons is empty!");
                return;
            }

            _receiveIncome.onClick.AddListener(HandleReceiveIncomeButtonClick);
            _receiveDoubleIncome.onClick.AddListener(HandleReceiveDoubleIncomeButtonClick);
        }

        private void OnDestroy()
        {
            _viewModel.Dispose();
            _disposables.Dispose();

            _receiveIncome.onClick.RemoveListener(HandleReceiveIncomeButtonClick);
            _receiveDoubleIncome.onClick.RemoveListener(HandleReceiveDoubleIncomeButtonClick);
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as OfflineIncomeViewModel;

            _viewModel.OfflineIncomeReceivedSignal.Subscribe(HandleReceivedOfflineIncome).AddTo(_disposables);
            _viewModel.OfflineHoursChangedSignal.Subscribe(HandleOfflineHoursChanged).AddTo(_disposables);
            _viewModel.OfflineIncomeChangedSignal.Subscribe(HandleOfflineIncomeChanged).AddTo(_disposables);
            _viewModel.CanBeOpenedSignal.Subscribe(HandleCanBeOpenedState).AddTo(_disposables);

            if (_viewModel.IsNewGame || !_isCanBeOpened)
            {
                Debug.Log("[Offline Income View] Can't be opened");
                return;
            }

            OpenWindow();
        }

        private void OpenWindow()
        {
            gameObject.SetActive(true);

            _rectTransform
                .DOScale(_defaultScale, _openCloseAnimDuration)
                .SetEase(Ease.InCubic);
        }

        private void HandleReceivedOfflineIncome(bool state)
        {
            if (!state)
                _isCanBeOpened = !state;

            _rectTransform
                .DOScale(_receivedScale, _openCloseAnimDuration)
                .SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    gameObject.SetActive(!state);
                });
        }

        private void HandleCanBeOpenedState(bool state)
        {
            _isCanBeOpened = state;

            if (!_isCanBeOpened || _viewModel == null || _viewModel.IsNewGame || gameObject.activeSelf)
                return;

            OpenWindow();
        }

        private void HandleOfflineIncomeChanged(string value) => _offlineIncome.text = _offlineIncome.text.Replace("{income}", $"<color=yellow>{value}</color>");

        private void HandleOfflineHoursChanged(string hours) => _offlineHours.text = _offlineHours.text.Replace("{hours}", $"<color=white>{hours}</color>");

        private void HandleReceiveIncomeButtonClick() => _viewModel.ReceiveOfflineIncome();

        private void HandleReceiveDoubleIncomeButtonClick() => _viewModel.ReceiveDoubleOfflineIncome();
    }
}