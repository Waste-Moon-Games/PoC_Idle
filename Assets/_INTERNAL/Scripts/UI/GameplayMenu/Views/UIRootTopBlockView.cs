using UnityEngine;

namespace UI.GameplayMenu.Views
{
    public class UIRootTopBlockView : MonoBehaviour
    {
        public void AttachView(Transform view) => view.SetParent(transform, false);
    }
}