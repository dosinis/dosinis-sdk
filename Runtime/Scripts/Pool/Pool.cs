using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Pool
{
    public class Pool<T> where T : class, IPooled
    {
        private static readonly Dictionary<Type, Pool<T>> pools = new Dictionary<Type, Pool<T>>();

        private readonly List<IPooled> pooledObjects = new List<IPooled>();

        private readonly IPooled sourceObject;

        private const int CAPACITY = 100;

        private Pool(IPooled pooled)
        {
            sourceObject = pooled;
            sourceObject.gameObject.SetActive(false);
        }

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
                return pooledObjects[0] as T;
            }
                
            T newObj = UnityEngine.Object.Instantiate(sourceObject.gameObject, sourceObject.gameObject.transform.parent).GetComponent<T>();

            newObj.gameObject.SetActive(true);

            pooledObjects.Add(newObj);

            return newObj;
        }
        
        public static Pool<T> Create(IPooled source)
        {
            var pool = new Pool<T>(source);

            pools.Add(source.GetType(), pool);
            return pool;
        }

        public static void Dispose(Pool<T> pool)
        {
            pools.Remove(pool.sourceObject.GetType());
        }

        public static Pool<T> GetPool()
        {
            foreach (var pool in pools)
            {
                if (pool.Value.sourceObject is T)
                {
                    return pool.Value;
                }
            }

            Debug.LogError($"Couldn't find pool of type {typeof(T).Name}. Did you forget to create it?");

            return default;
        }
    }
}