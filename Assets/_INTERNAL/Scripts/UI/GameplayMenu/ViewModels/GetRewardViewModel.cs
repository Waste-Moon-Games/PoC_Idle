using Common.MVVM;

namespace UI.GameplayMenu.ViewModels
{
    public class GetRewardViewModel: IViewModel
    {
        private GetRewardModel _model;

        public void BindModel(IModel model)
        {
            _model = model as GetRewardModel;
        }
    }
}