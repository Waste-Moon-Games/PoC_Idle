using Common.MVVM;
using UI.GameplayMenu.Animations;
using UnityEngine;

namespace UI.GameplayMenu.Views
{
    public class MainGameView : MonoBehaviour, IView
    {
        [SerializeField] private ClickableZoneView _clickableZoneView;

        public ClickableZoneView ClickableZone => _clickableZoneView;

        public void BindViewModel(IViewModel viewModel)
        {
            _clickableZoneView.BindViewModel(viewModel);
        }

        public void BindAnimationService(ClickAnimationsService animationService)
        {
            _clickableZoneView.BindAnimationService(animationService);
        }

        public void AttachView(GameObject view) => view.transform.SetParent(transform, false);
    }
}