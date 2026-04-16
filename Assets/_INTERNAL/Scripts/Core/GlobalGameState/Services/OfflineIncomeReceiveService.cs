using Core.AdsSystem;
using R3;
using UnityEngine;

namespace Core.GlobalGameState.Services
{
    public class OfflineIncomeReceiveService
    {
        private readonly Subject<bool> _offlineIncomeReceivedSignal = new();

        private readonly PlayerEconomyService _playerEconomyService;
        private readonly PlayerOfflineIncomeCalculatorService _playerOfflineIncomeCalculatorService;
        private readonly AdsSystemContext _adsContext;

        private float _offlineIncome;

        private bool _offlineIncomeReceived = false;
        private readonly bool _isNewGame = false;

        public bool IsNewGame => _isNewGame;

        public Observable<bool> OfflineIncomeReceivedSignal => _offlineIncomeReceivedSignal.AsObservable();

        public OfflineIncomeReceiveService(
            PlayerEconomyService playerEconomyService,
            PlayerOfflineIncomeCalculatorService calculatorService,
            AdsSystemContext adsSystemContext, bool isNewGame = false)
        {
            _playerEconomyService = playerEconomyService;
            _playerOfflineIncomeCalculatorService = calculatorService;
            _adsContext = adsSystemContext;

            _isNewGame = isNewGame;
        }

        public void PrepareOfflineIncome()
        {
            float incomePerSecond = _playerEconomyService.PassiveIncomeAmount;
            _offlineIncome = _playerOfflineIncomeCalculatorService.CalculateOfflineIcnome(incomePerSecond).Income;
        }

        public void ReceiveOfflineIncome(float multiplier = 1f)
        {
            float totalIncome = _offlineIncome * multiplier;
            _playerEconomyService.AddCoinsByOtherIcnome(totalIncome);
            _offlineIncomeReceived = true;
            _offlineIncomeReceivedSignal.OnNext(_offlineIncomeReceived);
        }

        public void ReceiveDoubleOfflineIncome()
        {
            _adsContext.ShowRewarded(() =>
            {
                ReceiveOfflineIncome(2f);
                _offlineIncomeReceived = true;
                _offlineIncomeReceivedSignal.OnNext(_offlineIncomeReceived);
            });
        }
    }
}