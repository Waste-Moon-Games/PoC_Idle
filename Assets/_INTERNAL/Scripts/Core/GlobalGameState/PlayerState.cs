using Core.GlobalGameState.Services;
using SO.PlayerConfigs;
using UnityEngine;

namespace Core.GlobalGameState
{
    public class PlayerState
    {
        private readonly PlayerEconomyService _playerEconomyService;
        private readonly PlayerUpgradeService _playerUpgradeService;
        private readonly PlayerBonusesService _playerBonusesService;
        private readonly ShopState _shopState;

        public PlayerEconomyService EconomyService => _playerEconomyService;
        public PlayerUpgradeService UpgradeService => _playerUpgradeService;
        public PlayerBonusesService BonusesService => _playerBonusesService;
        public ShopState ShopState => _shopState;

        public PlayerState()
        {
            var economyConfig = Resources.Load<MainEconomyConfig>("Configs/Economy/MainEconomyConfig");
            var playerConfig = Resources.Load<PlayerConfig>("Configs/Player/PlayerConfig");

            _playerBonusesService = new(playerConfig);
            _playerEconomyService = new(economyConfig, _playerBonusesService.BonusStateChanged, playerConfig.BonusClickMultiplier);
            _playerUpgradeService = new(_playerEconomyService);
            _shopState = new();
        }

        public void StartAsyncTasks()
        {
            _playerEconomyService.StartAsyncTasks();
            _playerBonusesService.StartAsyncDecreaseTask();
        }

        public void StopAsyncTasks()
        {
            _playerEconomyService.StopAsyncTasks();
            _playerBonusesService.StopAsyncDecreaseTask();
        }

        public void Dispose()
        {
            _playerEconomyService?.Dispose();
        }
    }
}