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

        private readonly float _initialTemporaryBonusDuration = 0f;

        private float _temporaryBonusDuration = 0f;

        private bool _temporaryBonusState = false;

        public Observable<bool> TemporaryBonusStateChanged => _temporaryBonusStateChangedSignal.AsObservable();
        public Observable<float> CurrencyBonusGiven => _giveCurrencyBonusSignal.AsObservable();
        public Observable<float> TemporaryBonusTimerChanged => _temporaryBonusTimerChangedSignal.AsObservable();

        public PlayerRewardedBonusesService(float initialTemporaryBonusDurationMinutes)
        {
            _initialTemporaryBonusDuration = initialTemporaryBonusDurationMinutes / 60f;

            _temporaryBonusDuration = _initialTemporaryBonusDuration * 60f;

            _temporaryBonusStateChangedSignal = new(_temporaryBonusState);
            _temporaryBonusTimerChangedSignal = new(_temporaryBonusDuration);
        }

        public void ActiveTemporaryBonus()
        {
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

            _temporaryBonusDuration = _initialTemporaryBonusDuration * 60f;
            _temporaryBonusTimerChangedSignal.OnNext(_temporaryBonusDuration);
        }

        private async UniTask TemporaryBonusCountdown(CancellationToken token)
        {
            float timer = _temporaryBonusDuration;

            while (!token.IsCancellationRequested)
            {
                if (timer < 0f)
                    DeactivateTemporaryBonus();

                timer -= Time.deltaTime;
                _temporaryBonusTimerChangedSignal.OnNext(timer);

                await UniTask.Delay(TimeSpan.FromSeconds(1f), ignoreTimeScale: true, cancellationToken: token);
            }
        }
    }
}