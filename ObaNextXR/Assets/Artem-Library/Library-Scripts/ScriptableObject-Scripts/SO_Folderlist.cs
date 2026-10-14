using System;
using System.Collections.Generic;
using UnityEngine;

namespace Artem_Library.Library_Scripts.ScriptableObject_Scripts
{
    [CreateAssetMenu(fileName = "FolderStructureData", menuName = "Custom Data/Folder Structure List")]
    public class SO_Folderlist : ScriptableObject
    {
        [Serializable]
        public class Folder
        {
            public string folderName;
            public List<string> subfolders;
        }

        public List<Folder> folders = new List<Folder>();
    }
}
