using Artem_Library.Library_Scripts.ScriptableObject_Scripts;
using UnityEditor;
using UnityEngine;

namespace Artem_Library.Editor
{
    public class FolderStructureDrawer : EditorWindow
    {
        private const string PathFolder = "Assets/Artem-Library/Custom Menu's/Folders.asset";
    
        [MenuItem("Artem Tools/Setup Unity Folder Structure")]
        public static void GenerateFolders()
        {
            var folderStructureData =  AssetDatabase.LoadAssetAtPath<SO_Folderlist>(PathFolder);

            if (folderStructureData == null)
            {
                Debug.LogError("FolderStructureData not found. Create one in the project and add folder structure data.");
                return;
            }

            foreach (var folder in folderStructureData.folders)
            {
                var fullPath = Application.dataPath + "/" + folder.folderName;

                if (!AssetDatabase.IsValidFolder("Assets/" + folder.folderName))
                {
                    AssetDatabase.CreateFolder("Assets", folder.folderName);
                    Debug.Log("Created folder: " + folder.folderName);
                }
                else
                {
                    Debug.Log("Folder already exists: " + folder.folderName);
                }

                foreach (var subfolder in folder.subfolders)
                {
                    var subfolderPath = fullPath + "/" + subfolder;
                    if (!AssetDatabase.IsValidFolder("Assets/" + subfolderPath))
                    {
                        AssetDatabase.CreateFolder("Assets/" + folder.folderName, subfolder);
                        Debug.Log("Created subfolder: " + subfolderPath);
                    }
                    else
                    {
                        Debug.Log("Subfolder already exists: " + subfolderPath);
                    }
                }
            }

            AssetDatabase.Refresh();
        }

  
    }
}

