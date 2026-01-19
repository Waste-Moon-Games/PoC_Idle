using Common.MVVM;
using R3;
using UI.GameplayMenu.Animations;
using UI.GameplayMenu.ViewModels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.GameplayMenu.Views
{
    public class ClickableZoneView : MonoBehaviour, IView, IPointerDownHandler, IPointerUpHandler
    {
        private readonly CompositeDisposable _disposables = new();

        private MainGameViewModel _viewModel;
        private ClickAnimationsService _animationService;

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void Update()
        {
            _animationService?.RotateAnimation();
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as MainGameViewModel;
        }

        public void BindAnimationService(ClickAnimationsService animationsService)
        {
            _animationService = animationsService;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _animationService.OnClickUp();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _viewModel.Click();
            _animationService.OnClickDown();
        }
    }
}