using System;
using System.Collections.Generic;
using DosinisSDK.Core;
using DosinisSDK.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DosinisSDK.Pool
{
    public class Pool<T> where T : class, IPooled
    {
        private static readonly Dictionary<Type, Pool<T>> pools = new();
        private const int CAPACITY = 1000;
        
        private readonly List<IPooled> pooledObjects = new();
        private readonly IPooled sourceObject;

        private readonly GameObject parent;
        
        private Pool(IPooled pooled)
        {
            sourceObject = pooled;
            
            if (sourceObject.gameObject.IsInScene())
            {
                sourceObject.gameObject.SetActive(false);
            }
            
            parent = new GameObject($"POOL-{typeof(T).Name}");
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
                
            T newObj = Object.Instantiate(sourceObject.gameObject, sourceObject.gameObject.transform.parent ?
                sourceObject.gameObject.transform.parent : parent.transform).GetComponent<T>();

            newObj.gameObject.SetActive(true);

            if (newObj.gameObject.TryGetComponent(out IPoolInitialized init))
            {
                init.Initialize(App.Core);
            }
            
            pooledObjects.Add(newObj);

            return newObj;
        }
        
        public static Pool<T> GetOrCreate(IPooled source)
        {
            if (pools.TryGetValue(source.GetType(), out var pool))
            {
                return pool;
            }
            
            pool = new Pool<T>(source);
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