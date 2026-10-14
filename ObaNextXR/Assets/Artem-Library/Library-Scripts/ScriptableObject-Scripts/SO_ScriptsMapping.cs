using UnityEngine;

namespace Artem_Library.Library_Scripts.ScriptableObject_Scripts
{
    [CreateAssetMenu(fileName = "ScriptMapping", menuName = "Custom Data/ ScriptMapping", order = 1)]
    public class SO_ScriptsMapping : ScriptableObject
    {
        [System.Serializable]
        public class ScriptRule
        {
            public string scriptNameSuffix;
            public string folderName;
        }

        public ScriptRule[] rules;
    }
}