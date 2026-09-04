using UI.Common.Components;
using UnityEngine;

namespace UI.GameplayMenu.Views.Panels
{
    public class RewardsPanelView : PanelAnimations
    {
        void Awake()
        {
            if(_objectRectTransform == null)
                _objectRectTransform.GetComponent<RectTransform>();

            base.Initialize();
        }
    }
}