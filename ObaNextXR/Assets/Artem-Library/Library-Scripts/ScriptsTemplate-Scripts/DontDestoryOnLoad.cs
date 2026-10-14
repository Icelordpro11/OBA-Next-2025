
using UnityEngine;

namespace Artem_Library.Library_Scripts.ScriptsTemplate_Scripts
{
    public class DontDestroyOnLoad<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance != null) return instance;
                instance = FindObjectOfType<T>();

                if (instance != null) return instance;
                var singletonObject = new GameObject(typeof(T).Name);
                instance = singletonObject.AddComponent<T>();
                DontDestroyOnLoad(singletonObject);

                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance != null)
                Destroy(gameObject);
            else
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}


