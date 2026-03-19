using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Threading;

namespace Core.GlobalGameState.Services
{
    public class AutoSaveService
    {
        private CancellationTokenSource _autoSaveToken;
        private readonly float _saveDelay;

        private readonly Subject<Unit> _autoSaveSignal = new();

        public Observable<Unit> AutoSaveSignal => _autoSaveSignal.AsObservable();

        public AutoSaveService(float saveDelay, CancellationTokenSource token)
        {
            _saveDelay = saveDelay;
            _autoSaveToken = token;
        }

        public async UniTask AsyncAutoSave()
        {
            while (!_autoSaveToken.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_saveDelay), ignoreTimeScale: false, cancellationToken: _autoSaveToken.Token);
                _autoSaveSignal.OnNext(Unit.Default);
            }
        }

        public void Dispose()
        {
            _autoSaveToken?.Cancel();
            _autoSaveToken?.Dispose();
            _autoSaveToken = null;
        }
    }
}