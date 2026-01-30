using UI.GameplayMenu.Animations;
using UI.GameplayMenu.Views;
using UnityEngine;

namespace Entry.Local.Gameplay
{
    public class GameplayResourceLoader
    {
        public void LoadRootViews(out UIRootTopBlockView topRootView)
        {
            var topRootViewPrefab = Resources.Load<UIRootTopBlockView>("UI/GameplayMenu/UIBlocks/Root/UIRootTopBlockView");

            topRootView = Object.Instantiate(topRootViewPrefab);
        }

        public void LoadMainViews(out NavigationButtonsView navigationButtonsView)
        {
            var navigationButtonsViewPrefab = Resources.Load<NavigationButtonsView>("UI/GameplayMenu/UIBlocks/UIButtonsView");

            navigationButtonsView = Object.Instantiate(navigationButtonsViewPrefab);
        }

        public void LoadPlayableViews(out MainGameView mainGameView, out EconomyPlayerInfoView economyPlayerInfoView, out PlayerStatsView playerStatsView)
        {
            var mainGameViewPrefab = Resources.Load<MainGameView>("UI/GameplayMenu/UIBlocks/UIRootView");
            var economyPlayerInfoViewPrefab = Resources.Load<EconomyPlayerInfoView>("UI/GameplayMenu/UIBlocks/EconomyPlayerInfo");
            var playerStatsViewPrefab = Resources.Load<PlayerStatsView>("UI/GameplayMenu/UIBlocks/PlayerStatsInfoView");

            mainGameView = Object.Instantiate(mainGameViewPrefab);
            economyPlayerInfoView = Object.Instantiate(economyPlayerInfoViewPrefab);
            playerStatsView = Object.Instantiate(playerStatsViewPrefab);
        }
    }
}