using Common.MVVM;
using UnityEngine;
using UI.GameplayMenu.ViewModels;

namespace UI.GameplayMenu.Views
{
    public class GetRewardView: MonoBehaviour, IView
    {
        private GetRewardViewModel _viewModel;

        public void BindViewModel(IViewModel viewModel) => _viewModel = viewModel as GetRewardViewModel;
    }
}