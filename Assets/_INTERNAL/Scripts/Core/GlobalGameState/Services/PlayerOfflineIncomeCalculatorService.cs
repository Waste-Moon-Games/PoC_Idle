using Core.GlobalGameState.Player;
using R3;
using System;

#if UNITY_WEBGL
using YG;
#endif

namespace Core.GlobalGameState.Services
{
    public class PlayerOfflineIncomeCalculatorService
    {
        private readonly BehaviorSubject<bool> _incomeReceivedSignal;
        private readonly BehaviorSubject<float> _offlineIcnomeCalculatedSignal;
        private readonly BehaviorSubject<float> _offlineHoursCalculatedSignal;

        private readonly long _lastOnlineTime;
        private readonly long _maxOfflineSeconds;

        public Observable<float> OfflineIncomeCalculatedSignal => _offlineIcnomeCalculatedSignal.AsObservable();
        public Observable<float> OfflineHoursCalculatedSignal => _offlineHoursCalculatedSignal.AsObservable();

        public PlayerOfflineIncomeCalculatorService(int maxOfflineHours, long lastOnlineTime = 0)
        {
            _offlineIcnomeCalculatedSignal = new(0f);
            _offlineHoursCalculatedSignal = new(0f);

#if UNITY_WEBGL
            _lastOnlineTime = lastOnlineTime;
#elif UNITY_ANDROID
            _lastOnlineTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
#endif
            _maxOfflineSeconds = maxOfflineHours * 3600;
        }

        public OfflineIncomeResult CalculateOfflineIcnome(float incomePerSec)
        {
            long offlineSeconds = CalculateOfflineSeconds();

            float income = incomePerSec * offlineSeconds;

            if (float.IsNaN(income) || float.IsInfinity(income) || income < 0)
                income = 0;

            float offlineHours = offlineSeconds / 3600f;

            _offlineIcnomeCalculatedSignal.OnNext(income);
            _offlineHoursCalculatedSignal.OnNext(offlineHours);

            return new OfflineIncomeResult(offlineSeconds, income);
        }

        private long CalculateOfflineSeconds()
        {
            long nowMs = GetCurrentTimeMs();

            if(nowMs <= 0 || _lastOnlineTime <= 0)
                return 0;

            long deltaSec = (nowMs - _lastOnlineTime) / 1000;

            if(deltaSec < 0)
                deltaSec = 0;

            if(deltaSec > _maxOfflineSeconds)
                deltaSec = _maxOfflineSeconds;

            return deltaSec;
        }

        private long GetCurrentTimeMs()
        {
#if UNITY_WEBGL
            var serverTime = YG2.ServerTime();
            if(serverTime > 0)
                return serverTime;
#endif

            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}