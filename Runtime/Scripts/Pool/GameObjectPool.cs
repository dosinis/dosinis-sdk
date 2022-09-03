using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Pool
{
    public class GameObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject source;

        private readonly List<Component> pool = new List<Component>();

        private void Awake()
        {
            if (string.IsNullOrEmpty(source.scene.name) == false) // NOTE: way of checking if it's prefab or a scene object
            {
                source.gameObject.SetActive(false);
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
