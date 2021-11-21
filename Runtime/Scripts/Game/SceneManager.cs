using DosinisSDK.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Game
{
    public class SceneManager : BehaviourModule, ISceneManager, IProcessable
    {
        protected readonly List<Managed> gameElements = new List<Managed>();
        private readonly Dictionary<Type, ManagedSingleton> singletons = new Dictionary<Type, ManagedSingleton>();

        public override void Init(IApp app)
        {
            foreach (var ge in GetComponentsInChildren<Managed>(true))
            {
                ge.Init(this);

                gameElements.Add(ge);

                var singleton = ge as ManagedSingleton;

                if (singleton)
                {
                    var sType = singleton.GetType();

                    if (singletons.ContainsKey(sType))
                    {
                        LogError($"Found more than one {sType.Name} singleton. Please make sure to have only one such element per Game");
                        continue;
                    }

                    singletons.Add(sType, singleton);
                }
            }
        }

        public virtual void Process(float delta)
        {
            foreach(var gameElement in gameElements)
            {
                if (gameElement.Alive)
                {
                    gameElement.Process(delta);
                }
            }
        }

        protected Managed CreateGameElement(Managed gameElement, Vector3 position, Transform parent)
        {
            var instance = Instantiate(gameElement, parent);
            instance.gameObject.transform.position = position;
            instance.Init(this);
            gameElements.Add(instance);

            return instance;
        }

        public Managed CreateGameElement(Managed gameElement, Vector3 position)
        {
            if (gameElement == null)
            {
                LogError("Trying to create GameElement which source is null");
                return null;
            }
            
            return CreateGameElement(gameElement, position, transform);
        }

        public Managed CreateGameElement(GameObject source, Vector3 position)
        {
            if (source == null)
            {
                LogError("Trying to create GameElement which source is null");
                return null;
            }

            var gameElement = source.GetComponent<Managed>();

            if (gameElement)
            {
                return CreateGameElement(gameElement, position);
            }
            
            LogError($"Source {source.name} doesn't have GameElement! Have you forgot to assign it?");
            return null;
        }

        public void DestroyGameElement(Managed element)
        {
            if (element == null)
            {
                LogError("Trying to destroy game element which is null");
                return;
            }

            gameElements.Remove(element);
            element.Destruct();
        }

        public T GetSingletonOfType<T>() where T : ManagedSingleton
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
