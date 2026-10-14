using Artem_Library.Library_Scripts;
using Artem_Library.Library_Scripts.ScriptsTemplate_Scripts;
using UnityEditor;
using UnityEngine;

namespace Artem_Library.Editor
{
    [InitializeOnLoad]
    public class HierarchySplitters : Singleton<HierarchySplitters>
    {
        static HierarchySplitters()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchySplitterItemGUI;
        }

        private static void HierarchySplitterItemGUI(int instanceID, Rect selectionRect)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (obj == null) return;

            var isSplitter = obj.name.StartsWith("---", System.StringComparison.Ordinal);

            if (!isSplitter) return;
            // Draw custom background color for splitters
            EditorGUI.DrawRect(selectionRect, Color.black);

            // Draw custom splitter icon
            var icon = EditorGUIUtility.ObjectContent(obj, obj.GetType()).image;
            if (icon != null)
            {
                var iconRect = new Rect(selectionRect.x, selectionRect.y, selectionRect.height, selectionRect.height);
                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit, true);
            }

            // Draw custom label
            var labelRect = new Rect(selectionRect.x + selectionRect.height, selectionRect.y,
                selectionRect.width - selectionRect.height, selectionRect.height);
            EditorGUI.DropShadowLabel(labelRect, obj.name.Replace("---", "").ToUpperInvariant());

            // Draw a thin line under the GameObject
            var lineRect = new Rect(selectionRect.x, selectionRect.y + selectionRect.height - 1, selectionRect.width, 1);
            EditorGUI.DrawRect(lineRect, Color.white); // Default to white line color
        }
    }
}