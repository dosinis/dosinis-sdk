using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class SceneManager : BehaviourModule, ISceneManager, IProcessable
    {
        protected readonly List<Managed> managedElements = new List<Managed>();
        private readonly Dictionary<Type, ManagedSingleton> singletons = new Dictionary<Type, ManagedSingleton>();

        public override void Init(IApp app)
        {
            foreach (var ge in GetComponentsInChildren<Managed>(true)) // TODO: Make it check whole scene instead of children components
            {
                ge.Init(this);

                managedElements.Add(ge);

                var singleton = ge as ManagedSingleton;

                if (singleton)
                {
                    var sType = singleton.GetType();

                    if (singletons.ContainsKey(sType))
                    {
                        LogError($"Found more than one {sType.Name} singleton. Make sure you only have one such element per Scene");
                        continue;
                    }

                    singletons.Add(sType, singleton);
                }
            }
        }

        public virtual void Process(float delta)
        {
            foreach(var managed in managedElements)
            {
                if (managed.Alive)
                {
                    managed.Process(delta);
                }
            }
        }

        protected Managed CreateGameElement(Managed managed, Vector3 position, Transform parent)
        {
            var instance = Instantiate(managed, parent);
            instance.gameObject.transform.position = position;
            instance.Init(this);
            managedElements.Add(instance);

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

            managedElements.Remove(managed);
            managed.Destruct();
        }

        public T GetSingletonOfType<T>() where T : ManagedSingleton // Make it IManagedSingleton
        {
            if (singletons.TryGetValue(typeof(T), out ManagedSingleton element))
            {
                return (T) element;
            }

            LogError($"No Singleton {typeof(T).Name} is found!");
            return default;
        }
    }
}
