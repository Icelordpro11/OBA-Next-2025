using Artem_Library.Library_Scripts.Systems_Scripts.Logger_System;
using UnityEditor;
using UnityEngine;

namespace Artem_Library.Editor
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class CustomLogger_Drawer : UnityEditor.Editor
    {
        /// <summary>
        /// Method to add a logger to the selected GameObject
        /// </summary>
        private void AddCustomLogger(MonoBehaviour targetScript)
        {
            if (targetScript.GetComponentInChildren<AdvancedLogger_System>() is not null)
            {
                Debug.LogWarning($"{targetScript.gameObject.name} already has an AdvancedLogger_System component.");
                return;
            }

            var loggerParent = GameObject.Find("Loggers") ?? new GameObject("Loggers");
            var loggerObject = new GameObject($"{targetScript.name} - Logger");
            loggerObject.transform.SetParent(loggerParent.transform);

            loggerObject.AddComponent<AdvancedLogger_System>();
            Debug.Log("Custom Logger added to the GameObject.");
        }
    
        /// <summary>
        /// Adding the "Add Custom Logger" option to the component's right-click context menu
        /// </summary>
        [MenuItem("CONTEXT/MonoBehaviour/Add Logger")]
        private static void AddCustomLoggerContextMenu(MenuCommand command)
        {
            var targetScript = (MonoBehaviour)command.context;
            var editorInstance = CreateEditor(targetScript) as CustomLogger_Drawer;
            editorInstance?.AddCustomLogger(targetScript);
        }
    
    }
}

