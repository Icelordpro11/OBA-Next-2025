using System.IO;
using UnityEditor;
using UnityEngine;
using Artem_Library.Library_Scripts.ScriptableObject_Scripts;
using System.Linq;

namespace Artem_Library.Editor
{
    public class ScriptOrganizerDrawer : EditorWindow
    {
        private SO_ScriptsMapping[] _allMappings;
        private int _selectedMappingIndex;
        private const string BaseFolder = "Assets/_Scripts";

        [MenuItem("Artem Tools/Script Organizer %g")] // Ctrl + G
        public static void ShowWindow()
        {
            var window = GetWindow<ScriptOrganizerDrawer>("Script Organizer");
            window.Show();
        }

        private void OnEnable()
        {
            // Load all instances of SO_ScriptsMapping in the project
            _allMappings = AssetDatabase.FindAssets("t:SO_ScriptsMapping")
                .Select(guid => AssetDatabase.LoadAssetAtPath<SO_ScriptsMapping>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();

            // Reset selected index if needed
            if (_selectedMappingIndex >= _allMappings.Length)
                _selectedMappingIndex = 0;
        }

        private void OnGUI()
        {
            GUILayout.Label("Script Organizer", EditorStyles.boldLabel);

            if (_allMappings.Length == 0)
            {
                EditorGUILayout.HelpBox("No Scripts Mapping assets found in the project.", MessageType.Warning);
                return;
            }

            // Create a dropdown menu for selecting the SO_ScriptsMapping
            string[] mappingNames = _allMappings.Select(mapping => mapping.name).ToArray();
            _selectedMappingIndex = EditorGUILayout.Popup("Select Scripts Mapping", _selectedMappingIndex, mappingNames);

            var selectedMapping = _allMappings[_selectedMappingIndex];

            // Display the currently selected mapping name
            EditorGUILayout.LabelField("Selected Mapping", selectedMapping.name);

            if (!GUILayout.Button("Organize Scripts")) return;

            if (selectedMapping == null)
            {
                Debug.LogError("No Scripts Mapping selected.");
                return;
            }

            OrganizeScripts(selectedMapping);
        }

        private static void OrganizeScripts(SO_ScriptsMapping mapping)
        {
            if (!Directory.Exists(BaseFolder))
            {
                Debug.LogError($"Base folder '{BaseFolder}' does not exist.");
                return;
            }

            // Get all script files in base folder and subfolders
            var guids = AssetDatabase.FindAssets("t:Script", new[] { BaseFolder });

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var fileName = Path.GetFileNameWithoutExtension(path);

                foreach (var rule in mapping.rules)
                {
                    if (!fileName.EndsWith(rule.scriptNameSuffix)) continue;
                    
                    // Determine target folder within _Scripts folder
                    var targetFolder = Path.Combine(BaseFolder, rule.folderName);

                    if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);

                    var targetPath = Path.Combine(targetFolder, Path.GetFileName(path));
                    AssetDatabase.MoveAsset(path, targetPath);
                    
                    Debug.Log($"Moved '{fileName}' to '{targetFolder}'");
                    break; // Exit loop once the script has been moved
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
