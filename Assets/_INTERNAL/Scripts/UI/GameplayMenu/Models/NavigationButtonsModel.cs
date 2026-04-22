using Common.MVVM;
using Core.AudioSystemCommon;
using Core.GlobalGameState;
using R3;

namespace UI.GameplayMenu.Models
{
    public enum MainMenuEvents
    {
        ShopClicked, SettingsClicked
    }

    public class NavigationButtonsModel : IModel
    {
        private readonly Subject<MainMenuEvents> _actionSignal = new();
        private readonly AudioSystemService _audioSystemService;

        public Observable<MainMenuEvents> Actions => _actionSignal.AsObservable();

        public NavigationButtonsModel(AudioSystemService audioSystemService) => _audioSystemService = audioSystemService;

        /// <summary>
        /// Клик по кнопке Магазина
        /// </summary>
        public void ClickShopSignal()
        {
            _audioSystemService.PlaySoundByID(SoundsIds.ClickSound);
            _actionSignal.OnNext(MainMenuEvents.ShopClicked);
        }

        /// <summary>
        /// Клик по кнопке Настроек
        /// </summary>
        public void ClickSettingsSignal() 
        {
            _audioSystemService.PlaySoundByID(SoundsIds.ClickSound);
            _actionSignal.OnNext(MainMenuEvents.SettingsClicked);
        }
    }
}