using Common.MVVM;
using Core.GlobalGameState.Services;
using R3;

namespace UI.GameplayMenu.Models
{
    public class OfflineIncomeModel : IModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly OfflineIncomeReceiveService _service;

        private readonly BehaviorSubject<bool> _incomeHasReceivedSignal;
        private readonly BehaviorSubject<float> _offlineIcnomeChangedSignal;
        private readonly BehaviorSubject<float> _offlineHoursChangedSignal;

        private float _offlineHours;
        private float _offlineIncome;
        private bool _isIncomeReceived = false;

        public bool IsNewGame => _service.IsNewGame;

        public Observable<bool> OfflineIncomeReceivedSignal => _incomeHasReceivedSignal.AsObservable();

        public Observable<float> OfflineIncomeChangedSignal => _offlineIcnomeChangedSignal.AsObservable();

        public Observable<float> OfflineHoursChangedSignal => _offlineHoursChangedSignal.AsObservable();

        public OfflineIncomeModel(
            Observable<float> offlineIncomeSignal,
            Observable<float> offlineHoursSignal,
            OfflineIncomeReceiveService receiveService)
        {
            _incomeHasReceivedSignal = new(false);
            _offlineIcnomeChangedSignal = new(0f);
            _offlineHoursChangedSignal = new(0f);

            _service = receiveService;

            _service.OfflineIncomeReceivedSignal.Subscribe(HandleIncomeReceivedSignal).AddTo(_disposables);

            offlineIncomeSignal.Subscribe(HandleOfflineIncomeChanged).AddTo(_disposables);
            offlineHoursSignal.Subscribe(HandleOfflineHoursChanged).AddTo(_disposables);
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
            _offlineIncome = amount;

            _offlineIcnomeChangedSignal.OnNext(_offlineIncome);
        }

        private void HandleOfflineHoursChanged(float hours)
        {
            _offlineHours = hours;

            _offlineHoursChangedSignal.OnNext(_offlineHours);
        }
    }
}