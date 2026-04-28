using Common.MVVM;
using Core.GlobalGameState.Services;
using R3;
using SO.PlayerConfigs;
using System.Globalization;
using UnityEngine;
using Utils.Formatter;

namespace UI.GameplayMenu.Models
{
    public class OfflineIncomeModel : IModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly OfflineIncomeReceiveService _service;
        private readonly NumberFormatter _formatter;
        private readonly OfflineIncomeLocalizationConfig _localizationConfig;

        private readonly BehaviorSubject<bool> _incomeHasReceivedSignal;
        private readonly BehaviorSubject<bool> _canBeOpenedSignal;
        private readonly BehaviorSubject<string> _offlineHoursChangedSignal;
        private readonly BehaviorSubject<string> _offlineIncomeChangedSignal;
        private readonly BehaviorSubject<SystemLanguage> _currentLangSignal;

        private readonly BehaviorSubject<string> _offlineIncomeDescSignal;
        private readonly BehaviorSubject<string> _offlineHoursDescSignal;

        private string _offlineHours;
        private string _offlineIncome;
        private bool _isIncomeReceived = false;

        public bool IsNewGame => _service.IsNewGame;

        public Observable<bool> OfflineIncomeReceivedSignal => _incomeHasReceivedSignal.AsObservable();
        public Observable<bool> CanBeOpenedSignal => _canBeOpenedSignal.AsObservable();
        public Observable<string> OfflineIncomeChangedSignal => _offlineIncomeChangedSignal.AsObservable();
        public Observable<string> OfflineHoursChangedSignal => _offlineHoursChangedSignal.AsObservable();
        public Observable<string> OfflineIncomeDescSignal => _offlineIncomeDescSignal.AsObservable();
        public Observable<string> OfflineHoursDescSignal => _offlineHoursDescSignal.AsObservable();
        public Observable<SystemLanguage> CurrentLangSignal => _currentLangSignal.AsObservable();

        public OfflineIncomeModel(
            PlayerOfflineIncomeCalculatorService incomeCalculatorService,
            OfflineIncomeReceiveService receiveService,
            LocalizationService localizationService,
            NumberFormatter formatter,
            OfflineIncomeLocalizationConfig localizationConfig)
        {
            _formatter = formatter;
            _localizationConfig = localizationConfig;

            _incomeHasReceivedSignal = new(false);
            _canBeOpenedSignal = new(false);
            _offlineIncomeChangedSignal = new(string.Empty);
            _offlineHoursChangedSignal = new(string.Empty);

            _offlineIncomeDescSignal = new(string.Empty);
            _offlineHoursDescSignal = new(string.Empty);

            localizationService.LanguageChangedSignal.Subscribe(HandleCurrentDescription).AddTo(_disposables);
            _currentLangSignal = new(localizationService.CurrentLanguage);

            _service = receiveService;

            _service.OfflineIncomeReceivedSignal.Subscribe(HandleIncomeReceivedSignal).AddTo(_disposables);

            incomeCalculatorService.OfflineIncomeCalculatedSignal.Subscribe(HandleOfflineIncomeChanged).AddTo(_disposables);
            incomeCalculatorService.OfflineHoursCalculatedSignal.Subscribe(HandleOfflineHoursChanged).AddTo(_disposables);
        }

        public void ReceiveOfflineIncome() => _service.ReceiveOfflineIncome();
        public void ReceiveDoubleOfflineIncome() => _service.ReceiveDoubleOfflineIncome();

        public void Dispose() => _disposables.Dispose();

        private void HandleIncomeReceivedSignal(bool value)
        {
            if (value)
                _isIncomeReceived = value;

            _incomeHasReceivedSignal.OnNext(_isIncomeReceived);
        }

        private void HandleOfflineIncomeChanged(float amount)
        {
            _canBeOpenedSignal.OnNext(amount > 0);

            _offlineIncome = _formatter.FormatNumber(amount);
            _offlineIncomeChangedSignal.OnNext(_offlineIncome);
        }

        private void HandleOfflineHoursChanged(float hours)
        {
            _offlineHours = $"{hours.ToString("F1", CultureInfo.InvariantCulture)}";
            _offlineHoursChangedSignal.OnNext(_offlineHours);
        }

        private void HandleCurrentDescription(SystemLanguage lang)
        {
            var startMessage = _localizationConfig.StartMessageLocalizations.Get(lang);
            var offlineIncomeMessage = _localizationConfig.OfflineIncomeLocalizations.Get(lang);

            _offlineHoursDescSignal.OnNext(startMessage);
            _offlineIncomeDescSignal.OnNext(offlineIncomeMessage);
            _currentLangSignal.OnNext(lang);
        }
    }
}