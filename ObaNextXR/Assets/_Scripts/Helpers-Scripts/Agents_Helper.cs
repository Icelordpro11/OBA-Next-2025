using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Scripts
{
    public static class Agents_Helper 
    {
        public static bool IsJson(string input)
        {
            input = input.Trim();
            return (input.StartsWith("{") && input.EndsWith("}")) || (input.StartsWith("[") && input.EndsWith("]"));
        }
        
        public static void SaveJsonToFile(string json, string fileName)
        {
            try
            {
                var path = Path.Combine(Application.persistentDataPath, fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                var formatted = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json), Formatting.Indented);
                File.WriteAllText(path, formatted); // Save the formatted JSON
                Debug.Log($"JSON saved to: {path}");
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to save JSON: " + ex.Message);
            }
        }
        
        public static async UniTask SpeakAndSetText(string sentence)
        {
            await OpenAITTS_System.Instance.Speak_Async(sentence);
            NexiUI_System.Instance.SetDialogueText(sentence);
        }

        public static void StopAllHandDetections()
        {
            var handSystems = PPNLoader_System.Instance.ContentParent.GetComponentsInChildren<HandDetection_System>();
            
            foreach (var system in handSystems) system.StopHandDetection();
            
        }
    }
}