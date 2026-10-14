using System;
using UnityEngine;
using UnityEngine.Events;

namespace Artem_Library.Library_Scripts.Systems_Scripts.Scriptable_Events_Scripts.Example_S__Events
{
  /// <summary>
  /// When Raising the Event i can put data (int, float, string) with it and when an function need a data it take from the listener
  /// </summary>
  [Serializable]
  public class CustomGameEvent: UnityEvent<Component, object>{}

  /// <summary>
  /// Listener that Subscribe to the Scriptable Event and execute the Unity Event with the function
  /// </summary>
  public class GameEventListener : MonoBehaviour
  {
    [SerializeField] private GameEvent gameEvent;
    [SerializeField] private CustomGameEvent response;
  
    private void OnEnable() => gameEvent.RegisterListener(this);
    private void OnDisable() => gameEvent.UnRegisterListener(this);
    public void OnEventRaised(Component sender, object data) => response.Invoke(sender, data);
  }
}