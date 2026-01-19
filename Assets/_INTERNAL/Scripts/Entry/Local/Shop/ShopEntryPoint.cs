using Core.GlobalGameState;
using Core.Consts.Enums;
using Core.Shop.Base;
using Core.Shop.Models;
using R3;

using System.Collections.Generic;
using UnityEngine;
using Utils.DI;

using UI.ShopMenu.ViewModels;
using UI.ShopMenu.Views;

using UI.GameplayMenu.ViewModels;
using UI.GameplayMenu.Models;
using UI.GameplayMenu.Views;

using NavigationButtonsView = UI.ShopMenu.Views.NavigationButtonsView;
using NavigationButtonsViewModel = UI.ShopMenu.ViewModels.NavigationButtonsViewModel;
using NavigationButtonsModel = UI.ShopMenu.Models.NavigationButtonsModel;

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
                model.Dispose();

            _shopMenuModel.Dispose();
        }

        public Observable<ShopEvents> Run(DIContainer container)
        {
            CreateScene(container, out NavigationButtonsModel navigationButtonsModel);

            return navigationButtonsModel.Actions.Where(action => action == ShopEvents.Exit);
        }

        private void CreateScene(in DIContainer container, out NavigationButtonsModel navigationButtonsModel)
        {
            navigationButtonsModel = new();

            CreateModels(container,
                out ShopModel clickUpgradesModel,
                out ShopModel passiveUpgradesModel,
                out ShopModel prestigeUpgradesModel,
                out PlayerInfoModel playerInfoModel);
            CreateViewModels(
                out NavigationButtonsViewModel navigationViewModel,
                out ShopViewModel clickUpgradesViewModel,
                out ShopViewModel passiveUpgradesViewModel,
                out ShopViewModel prestigeUpgradesViewModel,
                out PlayerInfoViewModel playerInfoViewModel);
            CreateViews(out NavigationButtonsView navigationButtonsView,
                out ShopView clickUpgradesView,
                out ShopView passiveUpgradesView,
                out ShopView prestigeUpgradesView,
                out PlayerInfoView playerInfoView);

            _shopMenuModel = new(_shopModels, navigationButtonsModel.Actions);

            playerInfoViewModel.BindModel(playerInfoModel);
            clickUpgradesViewModel.BindModel(clickUpgradesModel);
            clickUpgradesView.BindViewModel(clickUpgradesViewModel);

            passiveUpgradesViewModel.BindModel(passiveUpgradesModel);
            passiveUpgradesView.BindViewModel(passiveUpgradesViewModel);

            prestigeUpgradesViewModel.BindModel(prestigeUpgradesModel);
            prestigeUpgradesView.BindViewModel(prestigeUpgradesViewModel);

            playerInfoView.BindFormatter(new());
            playerInfoView.BindViewModel(playerInfoViewModel);

            navigationViewModel.BindModel(navigationButtonsModel);
            navigationButtonsView.BindViewModel(navigationViewModel);
        }

        private void CreateModels(in DIContainer container,
            out ShopModel clickUpgradesModel,
            out ShopModel passiveUpgradesModel,
            out ShopModel prestigeUpgradesModel,
            out PlayerInfoModel playerInfoModel)
        {
            var playerState = container.Resolve<GameWorldState>().PlayerState;
            var shopState = playerState.ShopState;
            shopState.Dispose();
            shopState.InitSubscribes();

            clickUpgradesModel = new(
                playerState.UpgradeService, shopState, ShopIds.CLICK_UPGRADES,
                "Configs/Shop/ClickItemsConfig", "Configs/Shop/ClickUpgradesMultiplierConfig", true);
            passiveUpgradesModel = new(
                playerState.UpgradeService, shopState, ShopIds.PASSIVE_UPGRADES,
                "Configs/Shop/PassiveItemsConfig", "Configs/Shop/PassiveUpgradesMultiplierConfig", false);
            prestigeUpgradesModel = new(
                playerState.UpgradeService, shopState, ShopIds.PRESTIGE_UPGRADES,
                "Configs/Shop/PrestigeItemsConfig", "Configs/Shop/PrestigeUpgradesMultiplierConfig", false);

            _shopModels.Add(clickUpgradesModel);
            _shopModels.Add(passiveUpgradesModel);
            _shopModels.Add(prestigeUpgradesModel);

            playerInfoModel = new();

            var economyModel = container.Resolve<GameWorldState>().PlayerState.EconomyService;
            playerInfoModel.BindModel(economyModel);
        }

        private void CreateViewModels(out NavigationButtonsViewModel navigationViewModel,
            out ShopViewModel clickUpgradesViewModel,
            out ShopViewModel passiveUpgradesViewModel,
            out ShopViewModel prestigeUpgradesViewModel,
            out PlayerInfoViewModel playerInfoViewModel)
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
            out PlayerInfoView playerInfoView)
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