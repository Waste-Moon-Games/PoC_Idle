using Common.MVVM;
using UnityEngine;

namespace UI.GameplayMenu.Views
{
    public class UIRootTopBlockView : MonoBehaviour, IView
    {
        public void AttachView(Transform view) => view.SetParent(transform, false);

        public void BindViewModel(IViewModel viewModel) { }
    }
}