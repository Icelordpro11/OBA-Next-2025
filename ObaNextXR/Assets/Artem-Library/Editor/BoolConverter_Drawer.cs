using Artem_Library.Attribute_Scripts;
using UnityEditor;
using UnityEngine;

namespace Artem_Library.Editor
{
    [CustomPropertyDrawer(typeof(BoolConverterAttribute))]
    public class BoolConverterDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var originalColor = GUI.backgroundColor;
            if (property.propertyType == SerializedPropertyType.Boolean)
            {
                EditorGUI.BeginChangeCheck();

                var value = property.boolValue;
                var buttonColor = value ? Color.green : Color.red;
            
                GUI.backgroundColor = buttonColor;

                if (GUI.Button(position, label.text))
                {
                    value = !value;
                    property.boolValue = value;
                }
            
                GUI.backgroundColor = originalColor;

                if (EditorGUI.EndChangeCheck()) property.serializedObject.ApplyModifiedProperties();
            }
            else
                EditorGUI.LabelField(position, "ToggleButtonAttribute only works with boolean variables.");
        }
    }
}