using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Artem_Library.Editor
{
    public class SetIconDrawer : EditorWindow
    {
        private const string Path = "Assets/Set Icn...";

        private List<Texture2D> _iconsList;
        private int _selectedIcon;
        private const int IconsPerRow = 10; // Number of icons to display in each row
        private const float IconSize = 70; // Adjust this size as needed

        [MenuItem(Path, priority = 0)]
        private static void ShowWindow()
        {
            var window = GetWindow<SetIconDrawer>();
            window.titleContent = new GUIContent("Set Ico");
            window.Show();
        }

        [MenuItem(Path, validate = true)]
        public static bool ShowMenuItemValidation() => Selection.objects.All(asset => asset is MonoScript);

        private void OnGUI()
        {
            if (_iconsList == null)
            {
                _iconsList = new List<Texture2D>();
                var assetsGuids = AssetDatabase.FindAssets("t:texture2D l:ScriptIcon");

                foreach (var assetGuid in assetsGuids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(assetGuid);
                    _iconsList.Add(AssetDatabase.LoadAssetAtPath<Texture2D>(path));
                }
            }

            if (_iconsList is { Count: > 0 })
            {
                _selectedIcon = GUILayout.SelectionGrid(_selectedIcon, Array.Empty<GUIContent>(), _iconsList.Count, GUILayout.Width(IconsPerRow * IconSize));
                if (Event.current != null)
                {
                    if (Event.current.isKey)
                    {
                        switch (Event.current.keyCode)
                        {
                            case KeyCode.KeypadEnter or KeyCode.Return:
                                ApplyIcon(_iconsList[_selectedIcon]);
                                Close();
                                break;
                            case KeyCode.Escape:
                                Close();
                                break;
                        }
                    }
                    else if (Event.current.button == 0 && Event.current.clickCount == 2)
                    {
                        ApplyIcon(_iconsList[_selectedIcon]);
                        Close();
                    }
                }

                GUILayout.BeginHorizontal();
                for (int i = 0; i < _iconsList.Count; i++)
                {
                    if (i == _selectedIcon) GUI.color = Color.green; // Highlight the selected icon
                
                    if (GUILayout.Button(new GUIContent(_iconsList[i]), GUILayout.Width(IconSize), GUILayout.Height(IconSize))) _selectedIcon = i;
                
                    GUI.color = Color.white;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Apply", GUILayout.Width(100)))
                {
                    ApplyIcon(_iconsList[_selectedIcon]);
                    Close();
                }
                if (GUILayout.Button("Close", GUILayout.Width(100))) Close();
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("No icons to display");
                if (GUILayout.Button("Close", GUILayout.Width(100))) Close();
            }
        }

        private static void ApplyIcon(Texture2D icon)
        {
            AssetDatabase.StartAssetEditing();
            foreach (var asset in Selection.objects)
            {
                var path = AssetDatabase.GetAssetPath(asset);
                var monoImporter = AssetImporter.GetAtPath(path) as MonoImporter;
                if (monoImporter != null) monoImporter.SetIcon(icon);
                AssetDatabase.ImportAsset(path);
            }

            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }
    }
}