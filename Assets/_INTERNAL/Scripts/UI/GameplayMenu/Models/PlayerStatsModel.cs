using Common.MVVM;
using Core.GlobalGameState;

namespace UI.GameplayMenu.Models
{
    public class PlayerStatsModel : IModel
    {
        private readonly PlayerState _model;

        public PlayerStatsModel(PlayerState model)
        {
            _model = model;
        }
    }
}