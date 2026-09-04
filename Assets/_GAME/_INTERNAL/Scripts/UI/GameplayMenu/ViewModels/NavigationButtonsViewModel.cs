using Common.MVVM;
using UI.GameplayMenu.Models;

namespace UI.GameplayMenu.ViewModels
{
    public class NavigationButtonsViewModel : IViewModel
    {
        private NavigationButtonsModel _model;

        public void BindModel(IModel model)
        {
            _model = model as NavigationButtonsModel;
        }

        public void ClickShop() => _model.ClickShopSignal();
        public void ClickSettings() => _model.ClickSettingsSignal();
    }
}