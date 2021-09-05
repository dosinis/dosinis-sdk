using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        private List<IProcessable> processables = new List<IProcessable>();

        private int processablesSize = 0;

        public event Action<bool> OnAppPaused = paused => { };
        public event Action<bool> OnAppFocus = focus => { };
        public event Action OnAppInitialized = () => { };

        public ModulesRegistry ModulesRegistry { get; private set; }

        public static App Core;
        public static Timer Timer;

        public T GetCachedModule<T>() where T : class, IModule
        {
            var mType = typeof(T);

            if (cachedModules.TryGetValue(mType, out IModule module))
            {
                return (T)module;
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

            if (module is IProcessable)
            {
                processables.Add(module as IProcessable);
                processablesSize++;
            }

            Debug.Log($"Registered {mType.Name} successfully");
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void Awake()
        {
            Core = this;

            var timerObject = new GameObject();
            timerObject.transform.parent = transform;
            timerObject.name = nameof(Timer);
            Timer = timerObject.AddComponent<Timer>();

            Debug.Log("Registering Modules...");

            ModulesRegistry = GetComponent<ModulesRegistry>();

            if (ModulesRegistry)
            {
                ModulesRegistry.Init(this);
            }
            else
            {
                Debug.LogWarning("No ModulesRegistry found! Have you forgot to attach it to App?");
            }

            IBehaviourModule[] cachedBehaviourModules = new IBehaviourModule[0];

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

            OnAppInitialized();
        }

        private void Update()
        {
            for (int i = 0; i < processablesSize; i++)
            {
                processables[i].Process(Time.deltaTime);
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
