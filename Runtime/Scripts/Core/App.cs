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
        private readonly Dictionary<Type, IModule> cachedModules = new Dictionary<Type, IModule>();

        private List<IProcessable> processables = new List<IProcessable>();

        private int processablesSize = 0;

        private static bool initialized = false;

        public event Action<bool> OnAppPaused = paused => { };
        public event Action<bool> OnAppFocus = focus => { };
        public static event Action OnAppInitialized = () => { };

        public ModulesRegistry ModulesRegistry { get; private set; }

        public static App Core;
        public ITimer Timer => GetCachedModule<ITimer>();
        public ICoroutineManager Coroutine => GetCachedModule<ICoroutineManager>();

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

        public static void InitSignal(Action onInit)
        {
            if (initialized)
            { 
                onInit();
            }
            else
            {
                OnAppInitialized += onInit;
            }
        }

        public void CreateBehaviourModule<T>() where T : BehaviourModule
        {
            var moduleObject = new GameObject();
            moduleObject.transform.parent = transform;
            moduleObject.name = typeof(T).Name;
            T module = moduleObject.AddComponent<T>();

            RegisterModule(module);
        }

        private void Awake()
        {
            Core = this;

            Debug.Log("Registering Modules...");

            ModulesRegistry = GetComponent<ModulesRegistry>();

            CreateBehaviourModule<CoroutineManager>();
            RegisterModule(new Timer());

            if (ModulesRegistry)
            {
                ModulesRegistry.Init(this);
            }
            else
            {
                Debug.LogWarning("No ModulesRegistry found! Did you forget to attach it to the App?");
            }

            IBehaviourModule[] cachedBehaviourModules = FindObjectsOfType(typeof(BehaviourModule)) as IBehaviourModule[];

            if (cachedBehaviourModules == null)
            {
                return;
            }

            Array.Sort(cachedBehaviourModules, (IBehaviourModule x, IBehaviourModule y) =>
            {
                return x.InitOrder.CompareTo(y.InitOrder);
            });

            Debug.Log("Registering Behaviour Modules...");

            foreach (var module in cachedBehaviourModules)
            {
                if (module is ICoroutineManager)
                    continue;

                RegisterModule(module);
            }

            initialized = true;
            
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
