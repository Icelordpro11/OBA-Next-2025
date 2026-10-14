using UnityEngine;

namespace Artem_Library.Library_Scripts.Systems_Scripts.Scriptable_Events_Scripts
{
    public class EventUsage_Example : MonoBehaviour
    {
        [Header("Event")] [SerializeField]
        private GameEvent _gameEvent;
    
        private int other;
        private void Update()
        {
          //  if (!Input.GetKeyDown(KeyCode.Space)) return;
            other++;
            _gameEvent.Raise(this, other);
        }

  
    }
}
