using System;
using System.Collections.Generic;
using System.Linq;
using Artem_Library.Library_Scripts.ScriptsTemplate_Scripts;
using UnityEngine;

namespace _Scripts.Systems_Scripts
{
    public class ShelfFinder_System : Singleton_MonoBehaviour<ShelfFinder_System>
    {
        [SerializeField] private List<BookShelfZone> shelfmarkZones = new();
        
        public Transform GetTransformForBookshelf(string shelfmark)
        {
            if (string.IsNullOrEmpty(shelfmark)) return null;

            return (from zone in shelfmarkZones
                where shelfmark.ToLowerInvariant().Contains(zone.keyword.ToLowerInvariant())
                select zone.targetTransform).FirstOrDefault();
        }
    }
    [Serializable]
    public class BookShelfZone
    {
        public string keyword;
        public Transform targetTransform;
    }
}
