using UI.GameplayMenu.Views;
using UI.ShopMenu.Views;
using UnityEngine;
using Utils.CustomResourceLoader;
using NavigationButtonsView = UI.ShopMenu.Views.NavigationButtonsView;

namespace Entry.Local.Shop
{
    public class ShopResourceLoader
    {
        public void LoadNavigationView(out NavigationButtonsView navigationButtonsView)
        {
            var navigationViewPrefab = ResourceLoader.LoadOrThrow<NavigationButtonsView>("UI/ShopMenu/UIBlocks/UINavigationView");

            navigationButtonsView = Object.Instantiate(navigationViewPrefab);
        }

        public EconomyPlayerInfoView LoadPlayerInfoView()
        {
            var playerInfoViewPrefab = ResourceLoader.LoadOrThrow<EconomyPlayerInfoView>("UI/ShopMenu/UIBlocks/UIPlayerInfoView");

            return Object.Instantiate(playerInfoViewPrefab);
        }

        public ShopView LoadShopView(string path)
        {
            var shopViewPrefab = ResourceLoader.LoadOrThrow<ShopView>(path);

            return Object.Instantiate(shopViewPrefab);
        }

        public UIShopRootView LoadShopRootView()
        {
            var rootPrefab = ResourceLoader.LoadOrThrow<UIShopRootView>("UI/ShopMenu/UIBlocks/UIShopView");

            return Object.Instantiate(rootPrefab);
        }
    }
}