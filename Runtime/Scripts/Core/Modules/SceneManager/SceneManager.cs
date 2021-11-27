using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class SceneManager : BehaviourModule, ISceneManager, IProcessable
    {
        protected readonly Dictionary<Type, Managed> managedElements = new Dictionary<Type, Managed>();

        public override void OnInit(IApp app)
        {
            foreach (var managed in FindObjectsOfType<Managed>(true))
            {
                managed.Init(this);

                managedElements.Add(managed.GetType(), managed);

                if (managed.AutoInit)
                    managed.OnInit();
            }
        }

        public virtual void Process(float delta)
        {
            foreach(var managed in managedElements)
            {
                if (managed.Value.Alive)
                {
                    managed.Value.Process(delta);
                }
            }
        }

        public T As<T>() where T : class, ISceneManager
        {
            return this as T;
        }

        public T GetManaged<T>() where T : class, IManaged
        {
            var type = typeof(T);

            managedElements.TryGetValue(type, out Managed managed);

            if (managed)
            {
                return managed as T;
            }

            foreach (var element in managedElements)
            {
                if (element.Value is T val)
                {
                    managedElements.Add(type, element.Value);
                    return val;
                }
            }

            LogError($"Managed of type {typeof(T).Name} is not found! Maybe it's not ready yet?");

            return default;
        }

        protected Managed CreateGameElement(Managed managed, Vector3 position, Transform parent)
        {
            var type = managed.GetType();

            if (managedElements.ContainsKey(type))
            {
                Debug.LogError($"Managed of type {type.Name} already exists");
                return null;
            }

            var instance = Instantiate(managed, parent);
            instance.gameObject.transform.position = position;
            instance.Init(this);

            managedElements.Add(type, instance);

            return instance;
        }

        public Managed CreateManagedElement(Managed managed, Vector3 position)
        {
            if (managed == null)
            {
                LogError("Trying to create GameElement which source is null");
                return null;
            }
            
            return CreateGameElement(managed, position, transform);
        }

        public Managed CreateManagedElement(GameObject source, Vector3 position)
        {
            if (source == null)
            {
                LogError("Trying to create GameElement which source is null");
                return null;
            }

            var managed = source.GetComponent<Managed>();

            if (managed)
            {
                return CreateManagedElement(managed, position);
            }
            
            LogError($"Source {source.name} doesn't have GameElement! Have you forgot to assign it?");
            return null;
        }

        public void DestroyManagedElement(Managed managed)
        {
            if (managed == null)
            {
                LogError("Trying to destroy game element which is null");
                return;
            }

            managedElements.Remove(managed.GetType());
            managed.Destruct();
        }
    }
}
