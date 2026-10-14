using System;
using System.Collections.Generic;
using UnityEngine;

namespace Artem_Library.Library_Scripts.ScriptableObject_Scripts
{
    [CreateAssetMenu(fileName = "Splitter", menuName = "Custom Data/ Splitter List", order = 0)]
    public class SO_CustomSplitter : ScriptableObject
    {
        public List<Custom_Splitter> _customSplitters;
    }
    
    [Serializable]
    public class Custom_Splitter
    {
        [field: SerializeField] public string SplitterName { get; set; }
        public Texture2D SplitterTexture;
    }
}