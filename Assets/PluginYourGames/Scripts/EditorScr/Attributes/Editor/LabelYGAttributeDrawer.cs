using UnityEditor;
using UnityEngine;
using YG.EditorScr;

namespace YG
{
    [CustomPropertyDrawer(typeof(LabelYGAttribute))]
    public class YGLabelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LabelYGAttribute attr = (LabelYGAttribute)attribute;

            GUIStyle labelStyle = TextStyles.Orange();

            if (attr.color == "white")
                labelStyle = TextStyles.White();
            else if (attr.color == "gray")
                labelStyle = TextStyles.Gray();
            else if (attr.color == "red")
                labelStyle = TextStyles.Red();
            else if (attr.color == "green")
                labelStyle = TextStyles.Green();

            EditorGUI.LabelField(position, attr.label, labelStyle);
        }
    }
}
