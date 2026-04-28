using Common.MVVM;
using R3;
using UI.GameplayMenu.Models;
using UnityEngine;

namespace UI.GameplayMenu.ViewModels
{
    public class OfflineIncomeViewModel : IViewModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly BehaviorSubject<string> _offlineHoursDescSignal;
        private readonly BehaviorSubject<string> _offlineIncomeDescSignal;

        private OfflineIncomeModel _model;

        private string _offlineHours;
        private string _offlineIncome;

        public bool IsNewGame => _model.IsNewGame;

        public Observable<bool> OfflineIncomeReceivedSignal => _model.OfflineIncomeReceivedSignal;
        public Observable<bool> CanBeOpenedSignal => _model.CanBeOpenedSignal;
        public Observable<SystemLanguage> CurrentLangSignal => _model.CurrentLangSignal;

        public Observable<string> OfflineHoursDescSignal => _offlineHoursDescSignal.AsObservable();
        public Observable<string> OfflineIncomeDescSignal => _offlineIncomeDescSignal.AsObservable();

        public OfflineIncomeViewModel()
        {
            _offlineHoursDescSignal = new(string.Empty);
            _offlineIncomeDescSignal = new(string.Empty);
        }

        public void BindModel(IModel model)
        {
            _model = model as OfflineIncomeModel;

            _model.OfflineIncomeChangedSignal.Subscribe(HandleChangedOfflineIncome).AddTo(_disposables);
            _model.OfflineHoursChangedSignal.Subscribe(HandleChangedOfflineHours).AddTo(_disposables);

            _model.OfflineHoursDescSignal.Subscribe(HandleHoursDescription).AddTo(_disposables);
            _model.OfflineIncomeDescSignal.Subscribe(HandleOfflineIncomeDescription).AddTo(_disposables);
        }

        public void ReceiveOfflineIncome() => _model.ReceiveOfflineIncome();
        public void ReceiveDoubleOfflineIncome() => _model.ReceiveDoubleOfflineIncome();

        public void Dispose()
        {
            _disposables.Dispose();
            _model.Dispose();
        }

        private void HandleChangedOfflineIncome(string formattedIncome) => _offlineIncome = formattedIncome;

        private void HandleChangedOfflineHours(string formattedHours) => _offlineHours = formattedHours;

        private void HandleHoursDescription(string startMessage)
        {
            var hoursMessage = startMessage.Replace("{hours}", $"<color=#00FFFF>{_offlineHours}</color>");

            _offlineHoursDescSignal.OnNext(hoursMessage);
        }

        private void HandleOfflineIncomeDescription(string desc)
        {
            var offlineMessage = desc.Replace("{income}", $"<color=yellow>{_offlineIncome}</color>");

            _offlineIncomeDescSignal.OnNext(offlineMessage);
        }
    }
}