using Common.MVVM;
using R3;
using System.Collections.Generic;
using UI.ShopMenu.ViewModels;
using UnityEngine;

namespace UI.ShopMenu.Views
{
    public class ShopView : MonoBehaviour, IView
    {
        private readonly CompositeDisposable _disposables = new();

        [SerializeField] private ItemView _prefab;
        [SerializeField] private Transform _container;

        private readonly List<ItemView> _items = new();

        private ShopViewModel _viewModel;

        private void OnDestroy() => _disposables.Dispose();

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as ShopViewModel;

            _viewModel.RequestedItems.Subscribe(HandleRequestedItems).AddTo(_disposables);
            _viewModel.StateChanged.Subscribe(HandleChangedState);
            _viewModel.RequestItems();
            _viewModel.RequestState();
        }

        private void HandleChangedState(bool state) => gameObject.SetActive(state);

        private void HandleRequestedItems(List<ItemViewModel> itemViewModels)
        {
            foreach (var itemViewModel in itemViewModels)
            {
                var itemView = Instantiate(_prefab, _container);
                itemView.BindViewModel(itemViewModel);
                _items.Add(itemView);
            }
        }
    }
}