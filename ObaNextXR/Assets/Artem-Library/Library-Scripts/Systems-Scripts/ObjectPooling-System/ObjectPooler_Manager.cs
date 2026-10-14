using System;
using System.Collections.Generic;
using Artem_Library.Attribute_Scripts;
using UnityEngine;

namespace Artem_Library.Library_Scripts.Systems_Scripts.ObjectPooling_System
{
    public class ObjectPooler_Manager : MonoBehaviour
    { 
        [SerializeField, ExpandableScript] private List<ObjectPoolData> _dataList; // list of scriptable object pools

        private Func<IPooledObject> _getComponent;
        private Dictionary<string, LinkedList<GameObject>> _poolDictionary; // dictionary that maps pool tags to linked lists of pooled objects
        private Dictionary<string, GameObject> _parentDictionary; // dictionary that maps pool tags to parent game objects

        private void Awake()
        {
            _poolDictionary = new Dictionary<string, LinkedList<GameObject>>();
            _parentDictionary = new Dictionary<string, GameObject>();

            // iterate through all the pools in the list
            foreach (var pool in _dataList)
            {
                var objectPool = new LinkedList<GameObject>();
                // create a new parent game object for the pool
                var parent = new GameObject(pool.tag + " Pool");
                _parentDictionary.Add(pool.tag, parent);

                // instantiate the objects for the pool and add them to the linked list
                for (var i = 0; i < pool.size; i++)
                {
                    var obj = Instantiate(pool.prefab, parent.transform);
                    obj.SetActive(false);
                    objectPool.AddLast(obj);
                }

                // add the linked list to the dictionary with the pool's tag as the key
                _poolDictionary.Add(pool.tag, objectPool);
            }
        }

        // method to spawn an object from the pool with the specified tag

        public GameObject SpawnFromPool(string tag, Action<GameObject> action)
        {
            // if the pool doesn't exist, return null
            var objectToSpawn = !_poolDictionary.ContainsKey(tag) ? null : _poolDictionary[tag].First;

            while (objectToSpawn != null && objectToSpawn.Value.activeInHierarchy) 
                objectToSpawn = objectToSpawn.Next;

            // if no inactive object was found, create a new one if the pool is not at its maximum size
            if (objectToSpawn == null)
            {
                // find the pool with the specified tag
                var pool = _dataList.Find(p => p.tag == tag);
                // if the pool is not at its maximum size, instantiate a new object and add it to the linked list
                if (pool.size < pool.maxSize)
                {
                    objectToSpawn = new LinkedListNode<GameObject>(Instantiate(pool.prefab, _parentDictionary[tag].transform));
                    _poolDictionary[tag].AddLast(objectToSpawn.Value);
                    pool.size++;
                }
                else
                {
                    // if the pool is at its maximum size, log a warning and return null
                    print("Pool with tag " + tag + " is at max size.");
                    return null;
                }
            }

            //activate the object and set its position and rotation
            objectToSpawn.Value.SetActive(true);
            action(objectToSpawn.Value);
          

            // if the object has an IPooledObject component, call its OnObjectSpawn method
            var pooledObj =  GetInterface(objectToSpawn.Value);
            pooledObj?.OnObjectSpawn();

            // return the spawned object
            return objectToSpawn.Value;
        }

        private IPooledObject GetInterface(GameObject node)
        {
            _getComponent = node.GetComponent<IPooledObject>;
            return _getComponent();
        }


// method to deactivate an object in the pool with the specified tag
        public void DeactivateObjectInPool(string tagname, GameObject obj)
        {
            if (_poolDictionary.ContainsKey(tagname))
            {
                if (!_poolDictionary[tagname].Contains(obj))
                {
                    print("Object is not in pool with tag " + tagname + ".");
                    return;
                }

                obj.SetActive(false);
            }
            else
            {
                print("Pool with tag " + tagname + " doesn't exist.");
                return;
            }
            
        }

// method to deactivate the first active object in the pool with the specified tag
        public void DeactivateFirstActiveObjectInPool(string tagname)
        {
         
            if (!_poolDictionary.ContainsKey(tagname))
            {
                print("Pool with tag " + tagname + " doesn't exist."); return;
            }
            
            var objectPool = _poolDictionary[tagname];
            var node = objectPool.First;
            
            while (node != null && !node.Value.activeInHierarchy) node = node.Next;
            
            node?.Value.SetActive(false);
        }

// method to clear the pool with the specified tag
        public void ClearPool(string tagname)
        {
            if (_poolDictionary.ContainsKey(tagname))
            {
                var objectPool = _poolDictionary[tagname];
                var node = objectPool.First;
                
                while (node != null)
                {
                    Destroy(node.Value);
                    node = node.Next;
                }

                objectPool.Clear();
                var pool = _dataList.Find(p => p.tag == tagname);
                pool.size = 0;
            }
            else
            {
                Debug.LogWarning("Pool with tag " + tagname + " doesn't exist.");
                return;
            }
            
        }
    }
}
