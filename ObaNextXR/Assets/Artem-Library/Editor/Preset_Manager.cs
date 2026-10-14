using System.Collections.Generic;
using System.Linq;
using Artem_Library.Library_Scripts.ScriptableObject_Scripts;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace Artem_Library.Library_Scripts.Managers_Scripts
{
   public abstract class Preset_Manager
   {
      private const string PathFolder = "Assets/Artem-Library/Custom Menu's";
   
      [MenuItem("Artem Tools/Setup Library Presets")]
      private static void SetupLibraryPresets()
      {
         var presetListPaths = AssetDatabase.FindAssets("t:SO_PresetList", new[] { PathFolder }).Select(AssetDatabase.GUIDToAssetPath).ToArray();

         foreach (var presetListPath in presetListPaths)
         {
            var presetData = AssetDatabase.LoadAssetAtPath<SO_PresetList>(presetListPath);

            if (presetData == null)
            {
               Debug.LogError("DataPreset not found at the specified asset path: " + presetListPath);
               continue;
            }

            foreach (var info in presetData.presetList)
            {
               var preset = info.preset;
               var type = preset.GetPresetType();
               var existingPresets = Preset.GetDefaultPresetsForType(type);
               var isDuplicate = existingPresets.Any(existingPreset => existingPreset.preset == preset && existingPreset.filter == info.filterName);

               if (isDuplicate) continue;
               var newList = new List<DefaultPreset>(existingPresets) { new DefaultPreset(info.filterName, preset) };
               Preset.SetDefaultPresetsForType(type, newList.ToArray());
            }
         }
      }
   }
}

