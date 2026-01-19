using Common.MVVM;
using R3;

namespace UI.GameplayMenu.Models
{
    public enum MainMenuEvents
    {
        ShopClicked, SettingsClicked
    }

    public class NavigationButtonsModel : IModel
    {
        private readonly Subject<MainMenuEvents> _actionSignal = new();

        public Observable<MainMenuEvents> Actions => _actionSignal.AsObservable();

        /// <summary>
        /// Клик по кнопке Магазина
        /// </summary>
        public void ClickShopSignal() => _actionSignal.OnNext(MainMenuEvents.ShopClicked);

        /// <summary>
        /// Клик по кнопке Настроек
        /// </summary>
        public void ClickSettingsSignal() => _actionSignal.OnNext(MainMenuEvents.SettingsClicked);
    }
}