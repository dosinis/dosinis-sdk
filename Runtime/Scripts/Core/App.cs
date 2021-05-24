using System;
using UnityEngine;

namespace DosinisSDK.Core
{
    internal enum ModulesInstallType
    {
        LookInResources,
        LookInScene,
        LookInBoth
    }

    public sealed class App : MonoBehaviour, IApp
    {
        [SerializeField] private ModulesInstallType modulesInstallType;

        private IBehaviourModule[] cachedBehaviourModules;

        public static App Core;

        private void Awake()
        {
            Core = this;

            switch (modulesInstallType)
            {
                case ModulesInstallType.LookInResources:

                    break;
                case ModulesInstallType.LookInScene:
                    cachedBehaviourModules = FindObjectsOfType(typeof(BehaviourModule)) as IBehaviourModule[];
                    break;
                case ModulesInstallType.LookInBoth:

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

        public T GetCachedBehaviourModule<T>() where T : class, IBehaviourModule
        {
            foreach (var module in cachedBehaviourModules)
            {
                if (module is T value)
                {
                    return value;
                }
            }

            Debug.LogError($"Cached Behaviour Module {typeof(T).Name} is not ready yet!");
            return default;
        }

    }
}