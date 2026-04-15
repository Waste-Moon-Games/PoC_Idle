using Common.MVVM;
using R3;
using System.Globalization;
using UI.GameplayMenu.Models;
using UnityEngine;
using Utils.Formatter;

namespace UI.GameplayMenu.ViewModels
{
    public class OfflineIncomeViewModel : IViewModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly BehaviorSubject<string> _offlineIncomeChangedSignal;
        private readonly BehaviorSubject<string> _offlineHoursChangedSignal;

        private readonly NumberFormatter _formatter;

        private OfflineIncomeModel _model;

        private string _offlineHours;
        private string _offlineIncome;

        public bool IsNewGame => _model.IsNewGame;

        public Observable<bool> OfflineIncomeReceivedSignal => _model.OfflineIncomeReceivedSignal;

        public Observable<string> OfflineIncomeChangedSignal => _offlineIncomeChangedSignal.AsObservable();
        public Observable<string> OfflineHoursChangedSignal => _offlineHoursChangedSignal.AsObservable();

        public OfflineIncomeViewModel(NumberFormatter formatter)
        {
            _formatter = formatter;

            _offlineIncomeChangedSignal = new(string.Empty);
            _offlineHoursChangedSignal = new(string.Empty);

            Debug.Log("[Offline Income View Model] Created");
        }

        public void BindModel(IModel model)
        {
            _model = model as OfflineIncomeModel;

            _model.OfflineIncomeChangedSignal.Subscribe(HandleChangedOfflineIncome).AddTo(_disposables);
            _model.OfflineHoursChangedSignal.Subscribe(HandleChangedOfflineHours).AddTo(_disposables);
        }

        public void ReceiveOfflineIncome() => _model.ReceiveOfflineIncome();
        public void ReceiveDoubleOfflineIncome() => _model.ReceiveDoubleOfflineIncome();

        public void Dispose()
        {
            _disposables.Dispose();
            _model.Dispose();
        }

        private void HandleChangedOfflineIncome(float amount)
        {
            _offlineIncome = _formatter?.FormatNumber(amount);
            _offlineIncomeChangedSignal.OnNext(_offlineIncome);
        }

        private void HandleChangedOfflineHours(float hours)
        {
            _offlineHours = $"{hours.ToString("F1", CultureInfo.InvariantCulture)}";
            _offlineHoursChangedSignal.OnNext(_offlineHours);
        }
    }
}