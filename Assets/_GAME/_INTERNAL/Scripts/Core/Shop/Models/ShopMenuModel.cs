using Common.MVVM;
using Core.Common.Command;
using Core.Common.Command.Shop;
using Core.Consts.Enums;
using Core.Shop.Base;
using Cysharp.Threading.Tasks;
using R3;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Shop.Models
{
    public class ShopMenuModel : IModel, ICommandInvoker
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

        public void Run(string receiverId)
        {
            if (!_models.TryGetValue(receiverId, out var changebleModel))
                throw new System.Exception($"Model with id {receiverId} not found!");

            var modelToClose = _models.Values.FirstOrDefault(model => model.IsOpened == true && model.ShopId != receiverId);
            var modelToOpen = _models.Values.FirstOrDefault(model => model.ShopId == receiverId);
            Debug.Log($"[Shop Menu Model] Model to Close / Model to Open: {modelToClose.ShopId}/{modelToOpen.ShopId}");

            var changeShopsViewCommand = new ChangeShopViewStateCommand(modelToClose, modelToOpen);
            changeShopsViewCommand.SetExecutionDelay(modelToClose.ShopOpenDuration);
            changeShopsViewCommand.Execute().Forget();
        }

        private void ChangeModelState(string id) => Run(id);

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