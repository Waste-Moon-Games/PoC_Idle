using UnityEditor;
using UnityEngine;
using YG.EditorScr;

namespace YG
{
    [CustomPropertyDrawer(typeof(HeaderYGAttribute))]
    public class YGHeaderDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            HeaderYGAttribute attr = (HeaderYGAttribute)attribute;

            GUIStyle headerStyle = TextStyles.Header();
            headerStyle.normal.textColor = attr.color;

            position.y += attr.indent / 2;
            EditorGUI.LabelField(position, attr.header, headerStyle);
        }

        public override float GetHeight()
        {
            HeaderYGAttribute attr = (HeaderYGAttribute)attribute;
            return base.GetHeight() + attr.indent;
        }
    }
}
