using Common.MVVM;
using Core.GlobalGameState;
using R3;

namespace UI.GameplayMenu.Models
{
    public class PlayerStatsModel : IModel
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly PlayerState _model;

        public PlayerStatsModel(PlayerState model)
        {
            _model = model;

            _model.BonusesService.LevelChanged.Subscribe().AddTo(_disposables);
            _model.BonusesService.CurrentExpChanged.Subscribe().AddTo(_disposables);
            _model.BonusesService.ExpToLevelUpChanged.Subscribe().AddTo(_disposables);

            _model.BonusesService.RequestDefaultLevelState();
        }
    }
}