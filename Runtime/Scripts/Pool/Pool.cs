using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Pool
{
    public class Pool<T> where T : class, IPooled
    {
        private static readonly List<Pool<T>> pools = new List<Pool<T>>();

        private readonly List<T> pooledObjects = new List<T>();

        private T sourceObject;

        private const int CAPACITY = 100;

        public T Take()
        {
            foreach (var poolObject in pooledObjects)
            {
                if (poolObject is T poolable && !poolable.gameObject.activeSelf)
                {
                    poolable.gameObject.SetActive(true);
                    return poolable;
                }
            }

            if (pooledObjects.Count + 1 >= CAPACITY)
            {
                Debug.LogWarning($"Reached {typeof(T).Name} Pool capacity");
                return pooledObjects[0];
            }
                
            T newObj = Object.Instantiate(sourceObject.gameObject, sourceObject.gameObject.transform.parent).GetComponent<T>();

            newObj.gameObject.SetActive(true);

            pooledObjects.Add(newObj);

            return newObj;
        }
        
        private void SetSource(T pooled)
        {
            sourceObject = pooled;
        }

        public static Pool<T> Create(T source)
        {
            var pool = new Pool<T>();
            pool.SetSource(source);

            pools.Add(pool);
            return pool;
        }

        public static void Dispose(Pool<T> pool)
        {
            pools.Remove(pool);
            //pool.Dispose();
        }

        public static Pool<T> GetPool()
        {
            foreach (var pool in pools)
            {
                if (pool is Pool<T>)
                {
                    return pool;
                }
            }

            Debug.LogError($"Couldn't find pool {typeof(T).Name}");

            return default;
        }
    }
}