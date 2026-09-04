using UnityEngine;

namespace UI.ShopMenu.Views
{
    public class UIShopRootView : MonoBehaviour
    {
        public void AttachView(Transform child) => child.SetParent(transform, false);
    }
}