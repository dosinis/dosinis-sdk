using System;
using System.Collections.Generic;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.Pool
{
    public class GameObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject source;

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
            if (source != null && source.IsInScene())
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

        public void Dispose()
        {
            if (gameObject)
            {
                DestroyImmediate(gameObject);
            }
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
                if (obj.gameObject.activeSelf)
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
            foreach (var obj in GetAll<T>())
            {
                if (match(obj))
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
                if (match(obj))
                {
                    return obj;
                }
            }
            
            return default;
        }

        public void Prewarm<T>(int amount) where T : Component
        {
            for (int i = 0; i < amount; i++)
            {
                ReturnToPool(Take<T>());
            }
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

            var newObj = Instantiate(source, transform).GetComponent<T>();
            pool.Add(newObj);

            newObj.gameObject.SetActive(true);
            return newObj;
        }
    }
}
