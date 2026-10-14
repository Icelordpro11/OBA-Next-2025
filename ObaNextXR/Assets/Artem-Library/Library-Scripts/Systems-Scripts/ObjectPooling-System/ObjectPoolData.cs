using UnityEngine;
using UnityEngine.Search;

namespace Artem_Library.Library_Scripts.Systems_Scripts.ObjectPooling_System
{
    [CreateAssetMenu(fileName = "ObjectPooling", menuName = "PoolingData", order = 0)]
    public class ObjectPoolData : ScriptableObject
    {
        
        [Tooltip("tag for the pool")] public string tag; 
        [Tooltip("prefab for the objects in the pool"), SearchContext("t: prefab", SearchViewFlags.GridView | SearchViewFlags.Centered | SearchViewFlags.OpenInspectorPreview)] public GameObject prefab; 
        [Tooltip("initial minSize of the pool")] public int size;
        [Tooltip("maximum minSize of the pool (0 for unlimited)")] public int maxSize;
       
       
        private void OnValidate()
        {
            if (size <= maxSize) return;
            maxSize = size;
        }

    }
}
