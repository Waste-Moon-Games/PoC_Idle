using R3;

namespace Core.Common.Animations
{
    public abstract class AnimationService
    {
        private readonly Subject<bool> _stateChangeSignal = new();

        public Observable<bool> StateChangeSignal => _stateChangeSignal.AsObservable();

        protected virtual void ChangeState(bool value) => _stateChangeSignal.OnNext(value);
        public virtual void OnClickDown() { }
        public virtual void OnClickUp() { }
        public virtual void ClickAnimation(string amount = null) { }
    }
}