using UnityEngine;

namespace Artem_Library.Library_Scripts.Systems_Scripts.Scriptable_Events_Scripts.Example_S__Events
{
    public class MultiSceneCode_Example : MonoBehaviour
    {
        public void Message(Component sender, object data)
        {
            if (data is int) Debug.Log(data.ToString());
        }
    }
}
