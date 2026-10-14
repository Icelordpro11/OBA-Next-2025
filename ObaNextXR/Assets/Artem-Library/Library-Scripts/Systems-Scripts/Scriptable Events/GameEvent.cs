using System.Collections.Generic;
using Artem_Library.Library_Scripts.Systems_Scripts.Scriptable_Events_Scripts.Example_S__Events;
using UnityEngine;

namespace Artem_Library.Library_Scripts.Systems_Scripts.Scriptable_Events_Scripts
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "Event/GameEvent", order = 2)]
    public class GameEvent :ScriptableObject
    {
        public List<GameEventListener> _Listeners = new List<GameEventListener>();

        public void Raise(Component sender, object data)
        {
            foreach (var t in _Listeners) t.OnEventRaised(sender, data);
        }

        public void RegisterListener(GameEventListener listener)
        {
            if (_Listeners.Contains(listener)) return;
            _Listeners.Add(listener);
        }
        public void UnRegisterListener(GameEventListener listener)
        {
            if (!_Listeners.Contains(listener)) return;
            _Listeners.Remove(listener);
        }
    


    }
}
