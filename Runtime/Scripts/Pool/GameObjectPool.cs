using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Pool
{
    public class GameObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject source;
        [SerializeField] private Transform parent;

        private readonly List<Component> pool = new();

        public static GameObjectPool Create(GameObject source)
        {
            var pool = new GameObject("POOL-" + source.name).AddComponent<GameObjectPool>();
            pool.SetSource(source);
            return pool;
        }
        
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            if (source != null && string.IsNullOrEmpty(source.scene.name) == false) // NOTE: way of checking if it's prefab or a scene object
            {
                source.SetActive(false);
            }
        }

        private void SetSource(GameObject source)
        {
            this.source = source;
            Init();
        }

        public void ReturnAll()
        {
            foreach (var obj in pool)
            {
                obj.gameObject.SetActive(false);
            }
        }

        public void CleanUp()
        {
            foreach (var obj in pool)
            {
                Destroy(obj.gameObject);
            }

            pool.Clear();
        }

        public IEnumerable<T> GetAll<T>() where T : Component
        {
            foreach (var obj in pool)
            {
                yield return obj as T;
            }
        }

        public IEnumerable<T> GetAllActive<T>() where T : Component
        {
            foreach (var obj in pool)
            {
                if (obj.gameObject.activeInHierarchy)
                {
                    yield return obj as T;
                }
            }
        }
        
        public void ReturnToPool(Component obj)
        {
            if (pool.Contains(obj))
            {
                obj.gameObject.SetActive(false);
            }
        }
        
        public void ReturnToPoolIfFound<T>(Predicate<T> match) where T : Component
        {
            ReturnToPool(Find(match));
        }

        public bool Contains<T>(Predicate<T> match) where T : Component
        {
            foreach (var obj in pool)
            {
                var result = obj as T;
                
                if (match(result))
                {
                    return true;
                }
            }
            
            return false;
        }
        
        public T Find<T>(Predicate<T> match, bool lookActiveOnly = true) where T : Component
        {
            var list = lookActiveOnly ? GetAllActive<T>() : GetAll<T>();
            
            foreach (var obj in list)
            {
                var result = obj;
                
                if (match(result))
                {
                    return result;
                }
            }
            
            return default;
        }

        public T Take<T>() where T : Component
        {
            foreach (var obj in pool)
            {
                if (obj.gameObject.activeSelf == false)
                {
                    obj.gameObject.SetActive(true);
                    return obj as T;
                }
            }

            var newObj = Instantiate(source, parent ? parent : transform).GetComponent<T>();
            pool.Add(newObj);

            newObj.gameObject.SetActive(true);
            return newObj;
        }
    }
}
