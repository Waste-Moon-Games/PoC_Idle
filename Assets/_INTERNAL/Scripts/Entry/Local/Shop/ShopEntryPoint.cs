using Core.GlobalGameState;
using Core.Consts.Enums;
using Core.Shop.Base;
using Core.Shop.Models;
using R3;

using System.Collections.Generic;
using UnityEngine;
using Utils.DI;
using Utils.Formatter;

using UI.ShopMenu.ViewModels;
using UI.ShopMenu.Views;

using UI.GameplayMenu.ViewModels;
using UI.GameplayMenu.Models;
using UI.GameplayMenu.Views;

using NavigationButtonsView = UI.ShopMenu.Views.NavigationButtonsView;
using NavigationButtonsViewModel = UI.ShopMenu.ViewModels.NavigationButtonsViewModel;
using NavigationButtonsModel = UI.ShopMenu.Models.NavigationButtonsModel;
using System.Linq;
using Core.AdsSystem;

namespace Entry.Local.Shop
{
    public class ShopEntryPoint : MonoBehaviour
    {
        private readonly ShopResourceLoader _loader = new();
        private readonly List<ShopModel> _shopModels = new();

        private ShopMenuModel _shopMenuModel;

        private void OnDestroy()
        {
            foreach (ShopModel model in _shopModels)
                model.ItemsDisposablesClear();

            _shopMenuModel.Dispose();
        }

        public Observable<ShopEvents> Run(DIContainer container)
        {
            container.Resolve<AdsSystemContex>().ShowInterstitial();

            CreateScene(container, out NavigationButtonsModel navigationButtonsModel);

            return navigationButtonsModel.Actions.Where(action => action == ShopEvents.Exit);
        }

        private void CreateScene(in DIContainer container, out NavigationButtonsModel navigationButtonsModel)
        {
            navigationButtonsModel = new();
            var shopState = container.Resolve<GameWorldState>().PlayerState.ShopState;

            CreateModels(container,
                out EconomyPlayerInfoModel playerInfoModel);
            CreateViewModels(
                out NavigationButtonsViewModel navigationViewModel,
                out ShopViewModel clickUpgradesViewModel,
                out ShopViewModel passiveUpgradesViewModel,
                out ShopViewModel prestigeUpgradesViewModel,
                out EconomyPlayerInfoViewModel playerInfoViewModel);
            CreateViews(out NavigationButtonsView navigationButtonsView,
                out ShopView clickUpgradesView,
                out ShopView passiveUpgradesView,
                out ShopView prestigeUpgradesView,
                out EconomyPlayerInfoView playerInfoView);

            _shopModels.AddRange(shopState.ShopModels.Values);
            foreach (var shopModel in _shopModels)
                shopModel.SubscribeOnItems();

            _shopMenuModel = new(_shopModels, navigationButtonsModel.Actions);

            playerInfoViewModel.BindModel(playerInfoModel);

            var clickUpgradesModel = _shopModels.FirstOrDefault(x => x.ShopId == ShopIds.CLICK_UPGRADES);
            clickUpgradesViewModel.BindModel(clickUpgradesModel);
            clickUpgradesView.BindViewModel(clickUpgradesViewModel);

            var passiveUpgradesModel = _shopModels.FirstOrDefault(x => x.ShopId == ShopIds.PASSIVE_UPGRADES);
            passiveUpgradesViewModel.BindModel(passiveUpgradesModel);
            passiveUpgradesView.BindViewModel(passiveUpgradesViewModel);

            var prestigeUpgradesModel = _shopModels.FirstOrDefault(x => x.ShopId == ShopIds.PRESTIGE_UPGRADES);
            prestigeUpgradesViewModel.BindModel(prestigeUpgradesModel);
            prestigeUpgradesView.BindViewModel(prestigeUpgradesViewModel);

            playerInfoView.BindFormatter(new NumberFormatter());
            playerInfoView.BindViewModel(playerInfoViewModel);

            navigationViewModel.BindModel(navigationButtonsModel);
            navigationButtonsView.BindViewModel(navigationViewModel);
        }

        private void CreateModels(in DIContainer container,
            out EconomyPlayerInfoModel playerInfoModel)
        {
            playerInfoModel = new();

            var economyModel = container.Resolve<GameWorldState>().PlayerState.EconomyService;
            playerInfoModel.BindModel(economyModel);
        }

        private void CreateViewModels(out NavigationButtonsViewModel navigationViewModel,
            out ShopViewModel clickUpgradesViewModel,
            out ShopViewModel passiveUpgradesViewModel,
            out ShopViewModel prestigeUpgradesViewModel,
            out EconomyPlayerInfoViewModel playerInfoViewModel)
        {
            clickUpgradesViewModel = new();
            passiveUpgradesViewModel = new();
            prestigeUpgradesViewModel = new();
            playerInfoViewModel = new();
            navigationViewModel = new();
        }

        private void CreateViews(out NavigationButtonsView navigationButtonsView,
            out ShopView clickUpgradesView,
            out ShopView passiveUpgradesView,
            out ShopView prestigeUpgradesView,
            out EconomyPlayerInfoView playerInfoView)
        {
            _loader.LoadNavigationView(out NavigationButtonsView nView);

            var shopRootView = _loader.LoadShopRootView();

            clickUpgradesView = _loader.LoadShopView("UI/ShopMenu/UIBlocks/ClickUpgradesView");
            passiveUpgradesView = _loader.LoadShopView("UI/ShopMenu/UIBlocks/PassiveUpgradesView");
            prestigeUpgradesView = _loader.LoadShopView("UI/ShopMenu/UIBlocks/PrestigeUpgradesView");

            playerInfoView = _loader.LoadPlayerInfoView();
            navigationButtonsView = nView;

            shopRootView.AttachView(clickUpgradesView.transform);
            shopRootView.AttachView(passiveUpgradesView.transform);
            shopRootView.AttachView(prestigeUpgradesView.transform);
        }
    }
}