using Artem_Library.Attribute_Scripts;
using UnityEditor;
using UnityEngine;

namespace Artem_Library.Editor
{
    [CustomPropertyDrawer(typeof(ExpandableScriptAttribute))]
    public class ExpandableScriptDrawer : PropertyDrawer
    {
        private UnityEditor.Editor _editor;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            var foldoutRect = new Rect(position.x, position.y, 16, position.height);
            var propertyRect = new Rect(position.x + 16, position.y, position.width - 16, position.height);

            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none, true);
            EditorGUI.PropertyField(propertyRect, property, label, true);

            if (!property.isExpanded) return;
            EditorGUI.indentLevel++;

            var rect = EditorGUILayout.BeginVertical(GUI.skin.box);

            if (!_editor)
                UnityEditor.Editor.CreateCachedEditor(property.objectReferenceValue, null, ref _editor);
            _editor.OnInspectorGUI();

            EditorGUILayout.EndVertical();
            DrawOutlineBox(rect, Color.black, 1);

            EditorGUI.indentLevel--;
        }

        private static void DrawOutlineBox(Rect rect, Color color, int thickness)
        {
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
            EditorGUI.DrawRect(new Rect(rect.xMax- thickness, rect.y , thickness, rect.height), color);
        }
    }
}
