using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Core
{
    internal enum BehaviourModulesInstallType
    {
        LookInResources,
        LookInScene,
        LookInBoth
    }

    [RequireComponent(typeof(ModulesRegistry))]
    public sealed class App : MonoBehaviour, IApp
    {
        [SerializeField] private BehaviourModulesInstallType modulesInstallType;

        private readonly List<IModule> cachedModules = new List<IModule>();
        private IBehaviourModule[] cachedBehaviourModules;

        public event Action<bool> OnAppPaused = paused => { };
        public event Action<bool> OnAppFocus = focus => { };

        public static App Core;

        public T GetCachedModule<T>() where T : class, IModule
        {
            foreach (var module in cachedBehaviourModules)
            {
                if (module is T value)
                {
                    return value;
                }
            }

            foreach (var module in cachedModules)
            {
                if (module is T value)
                {
                    return value;
                }
            }

            Debug.LogError($"Cached Behaviour Module {typeof(T).Name} is not ready yet!");
            return default;
        }

        public void RegisterModule(IModule module)
        {
            if (cachedModules.Contains(module) == false)
            {
                module.Init(this);
                cachedModules.Add(module);
            }
            else
            {
                Debug.LogWarning($"App already contains module {nameof(module)}! Skipping...");
            }
        }

        private void Awake()
        {
            Core = this;

            GetComponent<ModulesRegistry>().Init(this);

            switch (modulesInstallType)
            {
                case BehaviourModulesInstallType.LookInResources:

                    break;
                case BehaviourModulesInstallType.LookInScene:
                    cachedBehaviourModules = FindObjectsOfType(typeof(BehaviourModule)) as IBehaviourModule[];
                    break;
                case BehaviourModulesInstallType.LookInBoth:

                    break;
                default:
                    Debug.LogError("Unknown module installation type");
                    break;
            }

            Array.Sort(cachedBehaviourModules, (IBehaviourModule x, IBehaviourModule y) =>
            {
                return x.InitOrder.CompareTo(y.InitOrder);
            });

            foreach (var module in cachedBehaviourModules)
            {
                try
                {
                    module.Init(this);
                    Debug.Log($"Module {module.GetType().Name} is initiallized successfully. Init Order Id: {module.InitOrder}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Module {module.GetType().Name} experienced error while initializing: {ex.Message}");
                }
            }
        }

        private void Update()
        {
            foreach (var module in cachedBehaviourModules)
            {
                module.Process(Time.deltaTime);
            }
        }

        private void OnApplicationPause(bool paused)
        {
            OnAppPaused(paused);
        }

        private void OnApplicationFocus(bool focus)
        {
            OnAppFocus(focus);
        }
    }
}