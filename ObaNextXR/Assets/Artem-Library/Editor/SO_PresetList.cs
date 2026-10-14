using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;

namespace Artem_Library.Library_Scripts.ScriptableObject_Scripts
{
    [CreateAssetMenu(fileName = "PresetMenu", menuName = "Custom Data/Preset Menu List", order = 0)]
    public class SO_PresetList : ScriptableObject
    {
        [System.Serializable]
        public class PresetInfo
        {
            public string filterName; 
            public Preset preset;
        }

        public List<PresetInfo> presetList = new List<PresetInfo>();
    }
}

