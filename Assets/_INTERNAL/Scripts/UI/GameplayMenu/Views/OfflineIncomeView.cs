using Common.MVVM;
using DG.Tweening;
using R3;
using TMPro;
using UI.GameplayMenu.ViewModels;
using UnityEngine;
using UnityEngine.UI;
using Utils.Localization;

namespace UI.GameplayMenu.Views
{
    public class OfflineIncomeView : MonoBehaviour, IView
    {
        private readonly CompositeDisposable _disposables = new();

        [Header("Text setup")]
        [SerializeField] private TextMeshProUGUI _offlineHours;
        [SerializeField] private TextMeshProUGUI _offlineIncome;
        [SerializeField] private TextMeshProUGUI _getText;
        [SerializeField] private TextMeshProUGUI _doubleGetText;

        [Space(5), Header("Buttons setup")]
        [SerializeField] private Button _receiveIncome;
        [SerializeField] private Button _receiveDoubleIncome;

        [Space(5), Header("Animation setup")]
        [SerializeField] private float _openAnimDuration = 1f;
        [SerializeField] private float _closeAnimDuration = 0.25f;
        [SerializeField] private Vector2 _receivedScale = new(0f, 0f);
        [SerializeField] private Vector2 _defaultScale = Vector2.one;

        [Space(5), Header("Buttons text localization setup")]
        [SerializeField] private LocalizedText _getLocalizations;
        [SerializeField] private LocalizedText _doubleGetLocalizations;

        private OfflineIncomeViewModel _viewModel;

        private bool _isCanBeOpened;
        private bool _isReceived;

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

            _viewModel.CanBeOpenedSignal.Subscribe(HandleCanBeOpenedSignal).AddTo(_disposables);
            _viewModel.CurrentLangSignal.Subscribe(HandleCurrentLang).AddTo(_disposables);

            _viewModel.OfflineIncomeReceivedSignal.Subscribe(HandleReceivedOfflineIncome).AddTo(_disposables);
            _viewModel.OfflineHoursDescSignal.Subscribe(HandleOfflineHoursChanged).AddTo(_disposables);
            _viewModel.OfflineIncomeDescSignal.Subscribe(HandleOfflineIncomeChanged).AddTo(_disposables);

            if (_viewModel.IsNewGame || !_isCanBeOpened || _isReceived)
                return;

            OpenWindow();
        }

        private void HandleCurrentLang(SystemLanguage lang)
        {
            _getText.text = _getLocalizations.Get(lang);
            _doubleGetText.text = _doubleGetLocalizations.Get(lang);
        }

        private void OpenWindow()
        {
            gameObject.SetActive(true);

            _rectTransform
                .DOScale(_defaultScale, _openAnimDuration)
                .SetEase(Ease.InOutBack);
        }

        private void HandleReceivedOfflineIncome(bool state)
        {
            if (state)
            {
                _isReceived = state;

                _rectTransform
                .DOScale(_receivedScale, _closeAnimDuration)
                .SetEase(Ease.OutFlash)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });

                return;
            }

            _isReceived = state;
        }

        private void HandleCanBeOpenedSignal(bool value) => _isCanBeOpened = value;

        private void HandleOfflineIncomeChanged(string desc) => _offlineIncome.text = desc;

        private void HandleOfflineHoursChanged(string des) => _offlineHours.text = des;

        private void HandleReceiveIncomeButtonClick() => _viewModel.ReceiveOfflineIncome();

        private void HandleReceiveDoubleIncomeButtonClick() => _viewModel.ReceiveDoubleOfflineIncome();
    }
}