using Artem_Library.Library_Scripts.ScriptableObject_Scripts;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Artem_Library.Editor
{
    public class HierarchySplitterDrawer: UnityEditor.Editor
    {
        private const string PathFolder = "Assets/Artem-Library/Custom Menu's/Standard-Splitters.asset";
        
        [MenuItem("GameObject/Create Standard Splitters ", false, 0)]
        private static void CustomMenuItem() => CreateCustomGameObjects();
    
        [MenuItem("Artem Tools/Setup Standard Splitters ")]
        private static void CustomSplitters() => CreateCustomGameObjects();

        private static void CreateCustomGameObjects()
        {
            var customSplitters = AssetDatabase.LoadAssetAtPath<SO_CustomSplitter>(PathFolder);

            if (customSplitters == null)
            {
                Debug.LogError("SO_CustomSplitters not found at the specified asset path. Please ensure the path is correct.");
                return;
            }

            foreach (var custom in customSplitters._customSplitters)
            {
                var gameObject = new GameObject("---" + custom.SplitterName);
                EditorGUIUtility.SetIconForObject(gameObject, custom.SplitterTexture);
            }
        }
    }
}
#endif

