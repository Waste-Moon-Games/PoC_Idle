using Cysharp.Threading.Tasks;

using R3;

using System;
using System.Threading;

using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.GlobalGameState.Services
{
    public class PlayerRewardBonusesService
    {
        private CancellationTokenSource _temporaryBonusCts;

        private readonly Subject<bool> _temporaryBonusStateChangedSignal = new();
        private readonly BehaviorSubject<float> _temporaryBonusActiveTimeChangedSignal;

        private readonly float _doubleActiveTimeDropChance = 0.15f;
        private readonly float _trippleActiveTimeDropChance = 0.1f;
        private readonly float _initialTemporaryBonusDuration = 0f;

        private float _temporaryBonusDuration = 0f;

        private bool _temporaryBonusState = false;

        public Observable<bool> TemporaryBonusStateChanged => _temporaryBonusStateChangedSignal.AsObservable();
        public Observable<float> TemporaryBonusActiveTimeChanged => _temporaryBonusActiveTimeChangedSignal.AsObservable();

        public PlayerRewardBonusesService(float initialTemporaryBonusDurationMinutes)
        {
            _initialTemporaryBonusDuration = initialTemporaryBonusDurationMinutes / 60f;

            _temporaryBonusDuration = _initialTemporaryBonusDuration * 60f;

            _temporaryBonusActiveTimeChangedSignal = new(_temporaryBonusDuration);
        }

        public void ActiveTemporaryBonus()
        {
            _temporaryBonusState = true;
            _temporaryBonusStateChangedSignal.OnNext(_temporaryBonusState);

            _temporaryBonusCts = new();
            TemporaryBonusCountdown(_temporaryBonusCts.Token).Forget();
        }

        private void DeactivateTemporaryBonus()
        {
            _temporaryBonusState = false;
            _temporaryBonusStateChangedSignal.OnNext(_temporaryBonusState);

            _temporaryBonusCts.Cancel();
            _temporaryBonusCts.Dispose();
            _temporaryBonusCts = null;

            _temporaryBonusDuration = _initialTemporaryBonusDuration;
            _temporaryBonusDuration *= RandRollTemporaryActiveBonusTimeMultiplier();
            _temporaryBonusActiveTimeChangedSignal.OnNext(_temporaryBonusDuration);
        }

        private float RandRollTemporaryActiveBonusTimeMultiplier()
        {
            float randRoll = Random.Range(0f, 1f);

            if (randRoll < _trippleActiveTimeDropChance)
                return 3f;
            if (randRoll < _doubleActiveTimeDropChance + _trippleActiveTimeDropChance)
                return 2f;

            return 1f;
        }

        private async UniTask TemporaryBonusCountdown(CancellationToken token)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_temporaryBonusDuration), ignoreTimeScale: true, cancellationToken: token);

                DeactivateTemporaryBonus();
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("[Player Reward Bonuses Service] Countdown was cancelled before deactivating temporary bonus!");
            }
        }
    }
}