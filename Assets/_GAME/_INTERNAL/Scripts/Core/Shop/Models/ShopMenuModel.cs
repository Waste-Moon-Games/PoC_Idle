using Common.MVVM;
using Core.Consts.Enums;
using Core.Shop.Base;
using R3;
using System.Collections.Generic;

namespace Core.Shop.Models
{
    public class ShopMenuModel : IModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly Dictionary<string, ShopModel> _models = new();

        public ShopMenuModel(List<ShopModel> models, Observable<ShopEvents> actions)
        {
            foreach (var model in models)
                _models.Add(model.ShopId, model);

            actions.Subscribe(HandleNavigationEvents).AddTo(_disposables);
        }

        public void Dispose() => _disposables.Dispose();

        private void ChangeModelState(string id)
        {
            if (!_models.TryGetValue(id, out var changebleModel))
                throw new System.Exception($"Model with id {id} not found!");

            changebleModel.Open();

            foreach (var model in _models.Values)
                if (model.ShopId != id)
                    model.Close();
        }

        private void HandleNavigationEvents(ShopEvents events)
        {
            switch (events)
            {
                case ShopEvents.ClickUpgrades:
                    ChangeModelState(ShopIds.CLICK_UPGRADES);
                    break;
                case ShopEvents.PassiveUpgrades:
                    ChangeModelState(ShopIds.PASSIVE_UPGRADES);
                    break;
                case ShopEvents.PrestigeUpgrades:
                    ChangeModelState(ShopIds.PRESTIGE_UPGRADES);
                    break;
            }
        }
    }
}