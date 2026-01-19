using Common.MVVM;
using UI.ShopMenu.Models;

namespace UI.ShopMenu.ViewModels
{
    public class NavigationButtonsViewModel : IViewModel
    {
        private NavigationButtonsModel _model;

        public void BindModel(IModel model)
        {
            _model = model as NavigationButtonsModel;
        }

        public void CloseShop() => _model.CloseShop();

        public void OpenClickUpgrades() => _model.OpenClickUpgrades();

        public void OpenPassiveUpgrades() => _model.OpenPassiveUpgrades();

        public void OpenPrestigeUpgrades() => _model.OpenPrestigeUpgrades();
    }
}