using UnityEditor;
using UnityEngine;

namespace YG
{
    [CustomPropertyDrawer(typeof(ColorYGAttribute))]
    public class ColorYGAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ColorYGAttribute colorAttribute = (ColorYGAttribute)attribute;
            Color previousColor = GUI.color;
            GUI.color = colorAttribute.color;

            if (property.propertyType == SerializedPropertyType.Generic)
                EditorGUI.PropertyField(position, property, label, true);
            else
                EditorGUI.PropertyField(position, property, label);

            GUI.color = previousColor;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
