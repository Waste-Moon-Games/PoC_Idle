using Common.MVVM;
using R3;
using UI.GameplayMenu.Models;

namespace UI.GameplayMenu.ViewModels
{
    public class MainGameViewModel : IViewModel
    {
        private MainGameModel _model;

        public void BindModel(IModel model)
        {
            _model = model as MainGameModel;
        }

        public void Click()
        {
            _model.AddCoins();
        }
    }
}