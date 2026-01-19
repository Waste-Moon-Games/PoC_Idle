using Common.MVVM;
using Core.GlobalGameState;
using Core.GlobalGameState.Services;

namespace UI.GameplayMenu.Models
{
    public class MainGameModel : IModel
    {
        private readonly PlayerEconomyService _model;

        public MainGameModel(PlayerEconomyService model)
        {
            _model = model;
        }

        /// <summary>
        /// Добавить монеты
        /// </summary>
        public void AddCoins() => _model.Add();
    }
}