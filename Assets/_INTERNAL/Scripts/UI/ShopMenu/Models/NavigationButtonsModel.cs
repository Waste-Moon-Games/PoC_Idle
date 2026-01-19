using Common.MVVM;
using Core.Consts.Enums;
using R3;

namespace UI.ShopMenu.Models
{
    public class NavigationButtonsModel : IModel
    {
        private readonly Subject<ShopEvents> _actionsSignal = new();

        public Observable<ShopEvents> Actions => _actionsSignal.AsObservable();

        /// <summary>
        /// Закрыть Магазин
        /// </summary>
        public void CloseShop() => _actionsSignal.OnNext(ShopEvents.Exit);

        /// <summary>
        /// Открыть Улучшение Кликов
        /// </summary>
        public void OpenClickUpgrades() => _actionsSignal.OnNext(ShopEvents.ClickUpgrades);

        /// <summary>
        /// Открыть Улучшение Idle-дохода
        /// </summary>
        public void OpenPassiveUpgrades() => _actionsSignal.OnNext(ShopEvents.PassiveUpgrades);

        /// <summary>
        /// Открыть Улучшение Престижа
        /// </summary>
        public void OpenPrestigeUpgrades() => _actionsSignal.OnNext(ShopEvents.PrestigeUpgrades);
    }
}