using UI.GameplayMenu.Animations;
using UI.GameplayMenu.Views;
using UnityEngine;

namespace Entry.Local.Gameplay
{
    public class GameplayResourceLoader
    {
        public void LoadMainViews(out NavigationButtonsView navigationButtonsView)
        {
            var navigationButtonsViewPrefab = Resources.Load<NavigationButtonsView>("UI/GameplayMenu/UIBlocks/UIButtonsView");

            navigationButtonsView = Object.Instantiate(navigationButtonsViewPrefab);
        }

        public void LoadPlayableViews(out MainGameView mainGameView, out PlayerInfoView playerInfoView)
        {
            var mainGameViewPrefab = Resources.Load<MainGameView>("UI/GameplayMenu/UIBlocks/UIRootView");
            var playerInfoViewPrefab = Resources.Load<PlayerInfoView>("UI/GameplayMenu/UIBlocks/UITopBlockView");

            mainGameView = Object.Instantiate(mainGameViewPrefab);
            playerInfoView = Object.Instantiate(playerInfoViewPrefab);
        }
    }
}