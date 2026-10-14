using System;
using Artem_Library.Attribute_Scripts;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Artem_Library.Library_Scripts.Systems_Scripts.Logger_System
{
    public class AdvancedLogger_System : MonoBehaviour
    {
        private enum LogLevel { Info, Warning, Error }

        [Header("Settings")]
        [SerializeField, BoolConverter] private bool _enableLogging = true;
        [SerializeField] private LogLevel _minimumLogLevel = LogLevel.Info;

        public void LogInfo(object message, Object context = null) => Log(message, LogLevel.Info, context);
        public void LogWarning(object message, Object context = null) => Log(message, LogLevel.Warning, context);
        public void LogError(object message, Object context = null) => Log(message, LogLevel.Error, context);

        [ContextMenu("Enable Logging")]
        public void Enable_Logging() => _enableLogging = true;

        [ContextMenu("Disable Logging")]
        public void Disable_Logging() => _enableLogging = false;

        [ContextMenu("Set Minimum Log Level to Info")]
        public void SetLogLevelInfo() => SetMinimumLogLevel(LogLevel.Info);

        [ContextMenu("Set Minimum Log Level to Warning")]
        public void SetLogLevelWarning() => SetMinimumLogLevel(LogLevel.Warning);

        [ContextMenu("Set Minimum Log Level to Error")]
        public void SetLogLevelError() => SetMinimumLogLevel(LogLevel.Error);

        private void SetMinimumLogLevel(LogLevel level) => _minimumLogLevel = level;

        private void Log(object message, LogLevel logLevel, Object context = null)
        {
            if (!_enableLogging || logLevel < _minimumLogLevel) return;

            var formattedMessage = FormatMessage(logLevel, message);

            switch (logLevel)
            {
                case LogLevel.Info:
                    Debug.Log(formattedMessage, context);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(formattedMessage, context);
                    break;
                case LogLevel.Error:
                    Debug.LogError(formattedMessage, context);
                    break;
                default:
                    Debug.LogError($"Unknown log level: {logLevel}");
                    break;
            }
        }

        private static string FormatMessage(LogLevel logLevel, object message) => $"[{DateTime.Now:HH:mm:ss}][{logLevel}] {message}";
    }
}