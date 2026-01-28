using Common.MVVM;
using R3;
using UI.GameplayMenu.Models;

namespace UI.GameplayMenu.ViewModels
{
    public class MainGameViewModel : IViewModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly Subject<float> _bonusGaugeChangedSignal = new();

        private MainGameModel _model;

        public Observable<float> ChangedBonusGauge => _bonusGaugeChangedSignal.AsObservable();

        public void BindModel(IModel model)
        {
            _model = model as MainGameModel;

            _model.BonusGaugeChange.Subscribe(HangleChangedBonusGauge).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _model.Dispose();
        }

        public void Click()
        {
            _model.Click();
        }

        private void HangleChangedBonusGauge(float amount) => _bonusGaugeChangedSignal.OnNext(amount);
    }
}