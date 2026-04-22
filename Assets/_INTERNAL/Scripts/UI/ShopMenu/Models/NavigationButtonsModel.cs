using Common.MVVM;
using Core.AudioSystemCommon;
using Core.Consts.Enums;
using Core.GlobalGameState;
using R3;

namespace UI.ShopMenu.Models
{
    public class NavigationButtonsModel : IModel
    {
        private readonly Subject<ShopEvents> _actionsSignal = new();
        private readonly AudioSystemService _audioSystemService;

        public Observable<ShopEvents> Actions => _actionsSignal.AsObservable();

        public NavigationButtonsModel(AudioSystemService audioSystemService) => _audioSystemService = audioSystemService;

        /// <summary>
        /// Закрыть Магазин
        /// </summary>
        public void CloseShop()
        {
            _audioSystemService.PlaySoundByID(SoundsIds.CloseSound);
            _actionsSignal.OnNext(ShopEvents.Exit);
        }

        /// <summary>
        /// Открыть Улучшение Кликов
        /// </summary>
        public void OpenClickUpgrades()
        {
            _audioSystemService.PlaySoundByID(SoundsIds.ClickSound);
            _actionsSignal.OnNext(ShopEvents.ClickUpgrades);
        }

        /// <summary>
        /// Открыть Улучшение Idle-дохода
        /// </summary>
        public void OpenPassiveUpgrades()
        {
            _audioSystemService.PlaySoundByID(SoundsIds.ClickSound);
            _actionsSignal.OnNext(ShopEvents.PassiveUpgrades);
        } 

        /// <summary>
        /// Открыть Улучшение Престижа
        /// </summary>
        public void OpenPrestigeUpgrades()
        {
            _audioSystemService.PlaySoundByID(SoundsIds.ClickSound);
            _actionsSignal.OnNext(ShopEvents.PrestigeUpgrades);
        }
    }
}