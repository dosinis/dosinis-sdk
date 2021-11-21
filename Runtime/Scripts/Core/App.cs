using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Core
{
    [RequireComponent(typeof(ModulesRegistry))]
    public sealed class App : MonoBehaviour, IApp
    {
        private readonly Dictionary<Type, IModule> cachedModules = new Dictionary<Type, IModule>();

        private List<IProcessable> processables = new List<IProcessable>();

        private static bool initialized = false;

        private static event Action OnAppInitialized;

        public event Action<bool> OnAppPaused;
        public event Action<bool> OnAppFocus;
        public event Action OnAppQuit;

        public ModulesRegistry ModulesRegistry { get; private set; }
        public AppConfig Config { get; private set; }

        public ITimer Timer => GetCachedModule<ITimer>();
        public ICoroutineManager Coroutine => GetCachedModule<ICoroutineManager>();
        public ISceneManager SceneManager => GetCachedModule<ISceneManager>();

        public static App Core;

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

            Debug.LogError($"Cached Module {typeof(T).Name} is not found! Maybe it's not ready yet?");
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

            cachedModules.Add(mType, module);

            try
            {
                module.Init(this);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Module {mType.Name} encountered initialization error: {ex.Message}");
            }
            
            if (module is IProcessable)
            {
                processables.Add(module as IProcessable);
            }

            Debug.Log($"Registered {mType.Name} successfully");
        }

        public void Restart()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
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

        public static void Create(AppConfig config)
        {
            var app = new App();
            app.Init(config);
        }

        private void Init(AppConfig config)
        {
            if (Core)
            {
                Debug.LogWarning($"{nameof(App)} already exists. " +
                    $"Make sure there's only one instance of the {nameof(App)}");

                Destroy(this);
                return;
            }

            Core = this;

            DontDestroyOnLoad(Core);

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

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (oldScene, newScene) =>
            {
                Debug.Log($"Scene was changed from {oldScene.name} to {newScene.name}");

                if (cachedModules.ContainsKey(typeof(ISceneManager)))
                {
                    cachedModules.Remove(typeof(ISceneManager));
                }

                if (cachedModules.ContainsKey(typeof(SceneManager)))
                {
                    cachedModules.Remove(typeof(SceneManager));
                }

                var newSceneManager = FindObjectOfType<SceneManager>() as ISceneManager;

                if (newSceneManager != null)
                {
                    RegisterModule(newSceneManager);
                }
                else
                {
                    Debug.LogWarning($"{newScene.name} doesn't have {nameof(SceneManager)}");
                }
            };

            initialized = true;

            OnAppInitialized?.Invoke();

            Debug.Log($"{nameof(App)} initialized");
        }

        private void Update()
        {
            foreach (var processable in processables)
            {
                processable.Process(Time.deltaTime);
            }
        }

        private void OnApplicationPause(bool paused)
        {
            OnAppPaused?.Invoke(paused);
        }

        private void OnApplicationFocus(bool focus)
        {
            OnAppFocus?.Invoke(focus);
        }

        private void OnApplicationQuit()
        {
            OnAppQuit?.Invoke();
        }
    }
}
