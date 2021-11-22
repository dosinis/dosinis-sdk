using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class SceneManager : BehaviourModule, ISceneManager, IProcessable
    {
        protected readonly List<Managed> managedElements = new List<Managed>();

        public override void Init(IApp app, ModuleConfig config = null)
        {
            base.Init(app, config);

            foreach (var managed in FindObjectsOfType<Managed>(true))
            {
                managed.Init(this);

                managedElements.Add(managed);
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

        public T As<T>() where T : class, ISceneManager
        {
            return this as T;
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
    }
}
