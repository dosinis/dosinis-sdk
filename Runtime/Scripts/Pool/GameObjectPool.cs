using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Pool
{
    public class GameObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject source;
        [SerializeField] private Transform parent;

        private readonly List<Component> pool = new List<Component>();

        private void Awake()
        {
            if (string.IsNullOrEmpty(source.scene.name) == false) // NOTE: way of checking if it's prefab or a scene object
            {
                source.gameObject.SetActive(false);
            }
        }

        public void ReturnAll()
        {
            foreach (var obj in pool)
            {
                obj.gameObject.SetActive(false);
            }
        }

        public IEnumerable<T> CollectAll<T>() where T : Component
        {
            foreach (var obj in pool)
            {
                yield return obj as T;
            }
        }

        public T GetByRule<T>(Predicate<T> rule) where T : Component
        {
            foreach (var obj in pool)
            {
                var result = obj as T;
                
                if (rule(result))
                {
                    return result;
                }
            }

            Debug.LogError("Couldn't find object that matched the rule");
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
