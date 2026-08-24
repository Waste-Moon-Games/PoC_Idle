using Common.MVVM;
using Core.Consts.Enums;
using Core.Shop.Base;
using Core.Shop.Models;
using R3;

namespace UI.ShopMenu.Models
{
    public class NavigationButtonsModel : IModel
    {
        private readonly Subject<ShopEvents> _actionsSignal = new();
        private readonly ReactiveProperty<int> _activeShopOpenSignal = new();

        public Observable<int> ActiveShopOpenSignal => _activeShopOpenSignal.AsObservable();
        public Observable<ShopEvents> Actions => _actionsSignal.AsObservable();

        public NavigationButtonsModel(ShopModel activeShop)
        {
            if (activeShop.ShopId == ShopIds.CLICK_UPGRADES)
                _activeShopOpenSignal.OnNext(0);
            if (activeShop.ShopId == ShopIds.PASSIVE_UPGRADES)
                _activeShopOpenSignal.OnNext(1);
            if (activeShop.ShopId == ShopIds.PRESTIGE_UPGRADES)
                _activeShopOpenSignal.OnNext(2);
        }

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