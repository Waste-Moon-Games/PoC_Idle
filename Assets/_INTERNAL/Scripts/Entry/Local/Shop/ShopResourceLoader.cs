using UI.GameplayMenu.Views;
using UI.ShopMenu.Views;
using NavigationButtonsView = UI.ShopMenu.Views.NavigationButtonsView;
using UnityEngine;

namespace Entry.Local.Shop
{
    public class ShopResourceLoader
    {
        public void LoadNavigationView(out NavigationButtonsView navigationButtonsView)
        {
            var navigationViewPrefab = Resources.Load<NavigationButtonsView>("UI/ShopMenu/UIBlocks/UINavigationView");

            navigationButtonsView = Object.Instantiate(navigationViewPrefab);
        }

        public EconomyPlayerInfoView LoadPlayerInfoView()
        {
            var playerInfoViewPrefab = Resources.Load<EconomyPlayerInfoView>("UI/ShopMenu/UIBlocks/UIPlayerInfoView");

            return Object.Instantiate(playerInfoViewPrefab);
        }

        public ShopView LoadShopView(string path)
        {
            var shopViewPrefab = Resources.Load<ShopView>(path);

            return Object.Instantiate(shopViewPrefab);
        }

        public UIShopRootView LoadShopRootView()
        {
            var rootPrefab = Resources.Load<UIShopRootView>("UI/ShopMenu/UIBlocks/UIShopView");

            return Object.Instantiate(rootPrefab);
        }
    }
}