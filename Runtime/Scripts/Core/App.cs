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

        private readonly Dictionary<Type, IModule> cachedModules = new Dictionary<Type, IModule>();

        private IBehaviourModule[] cachedBehaviourModules = new IBehaviourModule[0];

        public event Action<bool> OnAppPaused = paused => { };
        public event Action<bool> OnAppFocus = focus => { };

        public static App Core;

        public T GetCachedModule<T>() where T : class, IModule
        {
            var mType = typeof(T);

            if (cachedModules.TryGetValue(mType, out IModule module))
            {
                return (T) module;
            }

            foreach (var m in cachedModules)
            {
                if (m.Value is T value)
                {
                    cachedModules.Add(mType, m.Value);
                    return value;
                }
            }

            Debug.LogError($"Cached Module {typeof(T).Name} is not found!");
            return default;
        }

        public void RegisterModule(IModule module)
        {
            var mType = module.GetType();

            if (cachedModules.ContainsKey(mType))
            {
                Debug.LogError($"Modules registry already contains {mType.Name} module");
                return;
            }

            try
            {
                module.Init(this);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Module {mType.Name} encountered initialization error: {ex.Message}");
            }

            cachedModules.Add(mType, module);

            Debug.Log($"Registered {mType.Name} successfully");
        }

        private void Awake()
        {
            Core = this;

            Debug.Log("Registering Modules...");

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

            Debug.Log("Registering Behaviour Modules...");

            foreach (var module in cachedBehaviourModules)
            {
                RegisterModule(module);
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