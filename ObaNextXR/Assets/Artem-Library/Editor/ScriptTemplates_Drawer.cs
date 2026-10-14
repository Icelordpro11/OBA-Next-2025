using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Artem_Library.Editor
{
    public static class ScriptTemplatesDrawer
    {
        private const string PathFolder = "Assets/Script Create/", PathFile = "Artem-Library/ScriptTemplates";
        private static void Path(string folder, string name)
        {
            var path = $"Assets/{folder}/{name}.cs.txt";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, $"New {name}.cs");
        }
        #region Singletons

        [MenuItem(PathFolder+"Singleton Type/Non MonoBehaviour Singleton", priority = 3)]
        public static void Create_Singleton() => Path(PathFile, "SingletonWithoutMonoBehaviour");

        //-----------------------------------------------------------------------------------------------------------------------------------------------
        [MenuItem(PathFolder+"Singleton Type/With MonoBehaviour Singleton", priority = 3)]
        public static void Create_SingletonMonoBehaviour() => Path(PathFile, "SingletonWithMonoBehaviour"); 
    
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        [MenuItem(PathFolder+"Singleton Type/DontDestroyOnLoad With MonoBehaviour ", priority = 3)]
        public static void Create_DontDestroyOnLoadMonoBehaviour() => Path(PathFile, "DontDestroyOnLoad");

        #endregion
        #region Unity Scripts
    
        [MenuItem(PathFolder+"MonoBehaviour", priority = 2), Shortcut(" ",KeyCode.M, ShortcutModifiers.Alt)]
        public static void Create_MonoBehaviour() => Path(PathFile,  "MonoBehaviour");

        //-----------------------------------------------------------------------------------------------------------------------------------------------
        [MenuItem(PathFolder+"ScriptableObject", priority = 2), Shortcut(" ",KeyCode.O, ShortcutModifiers.Alt)]
        public static void Create_ScriptableObject() => Path(PathFile, "ScriptableObject");

        #endregion
        #region Class Scripts

        [MenuItem(PathFolder+"Class Type/Class", priority = 4)]
        public static void Create_Class() => Path(PathFile,  "Class");
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        [MenuItem(PathFolder+"Class Type/Serializable Class", priority = 6)]
        public static void Create_SerializableClass() => Path(PathFile,  "SerializableClass");
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        [MenuItem(PathFolder+"Class Type/Selection Class", priority = 6)]
        public static void SelectionClass() => Path( PathFile,  "SelectionClass");
    
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        [MenuItem(PathFolder+"Class Type/Static Class", priority = 5)]
        public static void Create_StaticClass() => Path(PathFile, "StaticClass");
    
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        [MenuItem(PathFolder+"Class Type/Abstract Class", priority = 6)]
        public static void Create_AbstractClass() => Path(PathFile,  "AbstractClass");

        #endregion
        #region SO_DataPreset Scripts

        [MenuItem(PathFolder+"Data type/Struct", priority = 1), Shortcut(" ",KeyCode.S, ShortcutModifiers.Alt)]
        public static void Create_Struct() => Path(PathFile, "Struct");

        //-----------------------------------------------------------------------------------------------------------------------------------------------
        [MenuItem(PathFolder+"Data type/Enum", priority = 1)]
        public static void Create_Enum() => Path(PathFile,  "Enum");

        //-----------------------------------------------------------------------------------------------------------------------------------------------
        [MenuItem(PathFolder+"Data type/Interface", priority = 1)]
        public static void Create_Interface() => Path(PathFile,  "Interface");
    
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        [MenuItem(PathFolder+"Data type/GenericClass", priority = 1)]
        public static void Create_Generic() => Path( PathFile,  "GenericClass");
    
        #endregion
        #region Editor Scripts
    
        [MenuItem(PathFolder+"Editor Type/Editor Class", priority = 7)]
        public static void Create_EditorClass() => Path(PathFile,  "EditorClass");
    
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        [MenuItem(PathFolder+"Editor Type/EditorWindow Class", priority = 7)]
        public static void Create_EditorWindowClass() => Path(PathFile, "EditorWindowClass");
    
        //----------------------------------------------------------------------------------------
        [MenuItem(PathFolder+"Editor Type/Editor PropertyDrawer", priority = 7)]
        public static void Create_EditorPropertyClass() => Path(PathFile, "PropertyDrawer");
  
        #endregion
        #region Fsm Scripts

        [MenuItem(PathFolder+"Fsm Type/Fsm Controller", priority = 7)]
        public static void Create_FsmController() => Path(PathFile, "Fsm_Controller");
    
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        [MenuItem(PathFolder+"Fsm Type/State With Enter", priority = 8)]
        public static void Create_EnterState() => Path(PathFile, "State_Enter");
    
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        [MenuItem(PathFolder+"Fsm Type/State With Update", priority = 8)]
        public static void Create_UpdateState() => Path(PathFile, "State_Update");
    
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        [MenuItem(PathFolder+"Fsm Type/State With FixedUpdate", priority = 8)]
        public static void Create_FixedUpdateState() => Path(PathFile, "State_FixedUpdate");
    
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        [MenuItem(PathFolder+"Fsm Type/State With Exit", priority = 8)]
        public static void Create_ExitState() => Path(PathFile, "State_Exit");
    
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        [MenuItem(PathFolder+"Fsm Type/State With All Interface", priority = 8)]
        public static void Create_AllState() => Path(PathFile, "State_All");
        #endregion
    }
}
