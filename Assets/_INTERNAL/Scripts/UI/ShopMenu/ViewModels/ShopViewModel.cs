using Common.MVVM;
using Core.Shop.Base;
using Core.Shop.Models;
using R3;
using System.Collections.Generic;

namespace UI.ShopMenu.ViewModels
{
    public class ShopViewModel : IViewModel
    {
        private readonly Subject<List<ItemViewModel>> _requestedItemsSignal = new();
        private readonly Subject<bool> _stateChangeSignal = new();
        private readonly List<ItemViewModel> _items = new();

        private bool _state;
        private ShopModel _model;

        public Observable<List<ItemViewModel>> RequestedItems => _requestedItemsSignal.AsObservable();
        public Observable<bool> StateChanged => _stateChangeSignal.AsObservable();

        public void BindModel(IModel model)
        {
            _model = model as ShopModel;

            _model.InitializeItems();
            _model.SubscribeOnItems();

            _model.RequestedAvailableItems.Subscribe(HandleRequestedItems);
            _model.StateChange.Subscribe(HandleChangedState);
        }

        public void RequestItems() => _model.RequestItems();

        public void RequestState() => _model.RequestState();

        private void HandleChangedState(bool state)
        {
            _state = state;
            _stateChangeSignal.OnNext(_state);
        }

        private void HandleRequestedItems(Dictionary<int, ItemModel> itemModels)
        {
            foreach (var item in itemModels.Values)
            {
                var itemViewModel = new ItemViewModel();
                itemViewModel.BindModel(item);
                _items.Add(itemViewModel);
            }

            _requestedItemsSignal.OnNext(_items);
        }
    }
}