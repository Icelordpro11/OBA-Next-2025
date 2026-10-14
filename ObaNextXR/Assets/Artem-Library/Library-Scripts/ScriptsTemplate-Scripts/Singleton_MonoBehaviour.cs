using UnityEngine;

namespace Artem_Library.Library_Scripts.ScriptsTemplate_Scripts
{
    public class Singleton_MonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindObjectOfType<T>();

                if (_instance == null) CreateSingletonInstance();
                return _instance;
            }
        }

        private static void CreateSingletonInstance()
        {
            GameObject obj = new GameObject(typeof(T).Name);
            _instance = obj.AddComponent<T>();
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}
