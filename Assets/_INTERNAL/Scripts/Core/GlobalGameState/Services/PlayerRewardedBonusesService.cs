using Cysharp.Threading.Tasks;

using R3;
using System;
using System.Threading;

using UnityEngine;

namespace Core.GlobalGameState.Services
{
    public class PlayerRewardedBonusesService
    {
        private CancellationTokenSource _temporaryBonusCts;

        private readonly BehaviorSubject<bool> _temporaryBonusStateChangedSignal;
        private readonly Subject<float> _giveCurrencyBonusSignal = new();
        private readonly BehaviorSubject<float> _temporaryBonusTimerChangedSignal;

        private readonly float _initialTemporaryBonusDurationInSeconds = 0f;

        private float _temporaryBonusDurationInSeconds = 0f;

        private bool _temporaryBonusState = false;

        public float InitialTemporaryBonusDuration => _initialTemporaryBonusDurationInSeconds;

        public Observable<bool> TemporaryBonusStateChanged => _temporaryBonusStateChangedSignal.AsObservable();
        public Observable<float> CurrencyBonusGiven => _giveCurrencyBonusSignal.AsObservable();
        public Observable<float> TemporaryBonusTimerChanged => _temporaryBonusTimerChangedSignal.AsObservable();

        public PlayerRewardedBonusesService(float initialTemporaryBonusDurationMinutes)
        {
            _initialTemporaryBonusDurationInSeconds = initialTemporaryBonusDurationMinutes * 60f;

            _temporaryBonusDurationInSeconds = _initialTemporaryBonusDurationInSeconds;

            _temporaryBonusStateChangedSignal = new(_temporaryBonusState);
            _temporaryBonusTimerChangedSignal = new(_temporaryBonusDurationInSeconds);
        }

        public void ActiveTemporaryBonus()
        {
            if (_temporaryBonusState)
                return;

            _temporaryBonusState = true;
            _temporaryBonusStateChangedSignal.OnNext(_temporaryBonusState);

            _temporaryBonusCts = new();
            TemporaryBonusCountdown(_temporaryBonusCts.Token).Forget();
        }

        public void GiveCurrencyBonus(float amount)
        {
            _giveCurrencyBonusSignal.OnNext(amount);
        }

        private void DeactivateTemporaryBonus()
        {
            _temporaryBonusState = false;
            _temporaryBonusStateChangedSignal.OnNext(_temporaryBonusState);

            _temporaryBonusCts.Cancel();
            _temporaryBonusCts.Dispose();
            _temporaryBonusCts = null;

            _temporaryBonusDurationInSeconds = _initialTemporaryBonusDurationInSeconds;
            _temporaryBonusTimerChangedSignal.OnNext(_temporaryBonusDurationInSeconds);
        }

        private async UniTask TemporaryBonusCountdown(CancellationToken token)
        {
            float timer = _temporaryBonusDurationInSeconds;

            while (!token.IsCancellationRequested)
            {
                if (timer < 0f)
                {
                    DeactivateTemporaryBonus();
                    return;
                }

                timer -= 1f;
                _temporaryBonusDurationInSeconds = Mathf.Max(timer, 0f);
                _temporaryBonusTimerChangedSignal.OnNext(timer);

                await UniTask.Delay(TimeSpan.FromSeconds(1f), ignoreTimeScale: true, cancellationToken: token);
            }
        }
    }
}