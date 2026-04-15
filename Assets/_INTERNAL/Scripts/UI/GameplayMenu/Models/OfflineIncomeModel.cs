using Common.MVVM;
using Core.GlobalGameState.Services;
using R3;
using UnityEngine;

namespace UI.GameplayMenu.Models
{
    public class OfflineIncomeModel : IModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly OfflineIncomeReceiveService _service;

        private BehaviorSubject<float> _offlineIcnomeChangedSignal;
        private BehaviorSubject<float> _offlineHoursChangedSignal;

        private float _offlineHours;
        private float _offlineIncome;

        public bool IsNewGame => _service.IsNewGame;

        public Observable<bool> OfflineIncomeReceivedSignal => _service.OfflineIncomeReceivedSignal;

        public Observable<float> OfflineIncomeChangedSignal => _offlineIcnomeChangedSignal.AsObservable();
        public Observable<float> OfflineHoursChangedSignal => _offlineHoursChangedSignal.AsObservable();

        public OfflineIncomeModel(
            Observable<float> offlineIncomeSignal,
            Observable<float> offlineHoursSignal,
            OfflineIncomeReceiveService receiveService)
        {
            _service = receiveService;

            offlineIncomeSignal.Subscribe(HandleOfflineIncomeChanged).AddTo(_disposables);
            offlineHoursSignal.Subscribe(HandleOfflineHoursChanged).AddTo(_disposables);

            Debug.Log("[Offline Income Model] Created");
        }

        public void ReceiveOfflineIncome() => _service.ReceiveOfflineIncome();
        public void ReceiveDoubleOfflineIncome() => _service.ReceiveDoubleOfflineIncome();

        public void Dispose() => _disposables.Dispose();

        private void HandleOfflineIncomeChanged(float amount)
        {
            _offlineIncome = amount;

            if (_offlineIcnomeChangedSignal == null)
                _offlineIcnomeChangedSignal = new(_offlineIncome);
            else
                _offlineIcnomeChangedSignal.OnNext(_offlineIncome);
        }

        private void HandleOfflineHoursChanged(float hours)
        {
            _offlineHours = hours;

            if (_offlineHoursChangedSignal == null)
                _offlineHoursChangedSignal = new(_offlineHours);
            else
                _offlineHoursChangedSignal.OnNext(_offlineHours);
        }
    }
}