using Common.MVVM;
using R3;

namespace UI.GameplayMenu.Models
{
    public enum MainMenuEvents
    {
        ShopClicked, SettingsClicked, WheelOfLuckClicked
    }

    public class NavigationButtonsModel : IModel
    {
        private readonly Subject<MainMenuEvents> _actionSignal = new();

        public Observable<MainMenuEvents> Actions => _actionSignal.AsObservable();

        /// <summary>
        /// Shop Clicked Signal Invoke
        /// </summary>
        public void ClickShopSignal() => _actionSignal.OnNext(MainMenuEvents.ShopClicked);

        /// <summary>
        /// Settings Clicked Signal Invoke
        /// </summary>
        public void ClickSettingsSignal() => _actionSignal.OnNext(MainMenuEvents.SettingsClicked);

        /// <summary>
        /// Wheel Of Luck Clicked Signal Invoke
        /// </summary>
        public void ClickWheelOfLuckSignal() => _actionSignal.OnNext(MainMenuEvents.WheelOfLuckClicked);
    }
}