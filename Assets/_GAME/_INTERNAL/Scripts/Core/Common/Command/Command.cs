using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Core.Common.Command
{
    public abstract class Command
    {
        private Queue<ICommandReceiver> _receiversQueue = new();

        private float _commandExecutionDelay;

        public virtual void AddReceiverInQueue(ICommandReceiver receiver)
        {
            if(_receiversQueue.Contains(receiver))
                return;

            _receiversQueue.Enqueue(receiver);
        }

        public void SetExecutionDelay(float delay) => _commandExecutionDelay = delay;

        public virtual async UniTask Execute()
        {
            if(_commandExecutionDelay == 0f)
                _commandExecutionDelay = 0.75f;

            while (_receiversQueue.Count > 0)
            {
                var receiver = _receiversQueue.Dequeue();
                receiver.Operation();
                await UniTask.Delay(TimeSpan.FromSeconds(_commandExecutionDelay));
            }
        }
    }
}