using Core.GlobalGameState.Services;
using SO.EconomyConfigs;
using UnityEngine;
using Utils.ModCoroutines;

namespace Core.GlobalGameState
{
    public class PlayerState
    {
        private readonly PlayerEconomyService _playerEconomyService;
        private readonly PlayerUpgradeService _playerUpgradeService;
        private readonly ShopState _shopState;

        public PlayerEconomyService EconomyService => _playerEconomyService;
        public PlayerUpgradeService UpgradeService => _playerUpgradeService;
        public ShopState ShopState => _shopState;

        public PlayerState(Coroutines coroutines)
        {
            var config = Resources.Load<MainEconomyConfig>("Configs/Economy/MainEconomyConfig");

            _playerEconomyService = new(config, coroutines);
            _playerUpgradeService = new(_playerEconomyService);
            _shopState = new();
        }
    }
}