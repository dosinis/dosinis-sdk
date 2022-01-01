using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Core
{
    public sealed class App : MonoBehaviour, IApp
    {
        private readonly Dictionary<Type, IModule> cachedModules = new Dictionary<Type, IModule>();

        private List<IProcessable> processables = new List<IProcessable>();

        private AppConfig config;

        // Events

        public event Action<bool> OnAppPaused;
        public event Action<bool> OnAppFocus;
        public event Action OnAppQuit;

        // Modules

        public ITimer Timer => GetCachedModule<ITimer>();
        public ICoroutineManager Coroutine => GetCachedModule<ICoroutineManager>();
        public ISceneManager SceneManager => GetCachedModule<ISceneManager>();

        // Static

        public static bool Initialized { get; private set; }
        private static Action OnAppInitialized;

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

        public void RegisterModule(IModule module, ModuleConfig mConfig = null)
        {
            var mType = module.GetType();

            if (cachedModules.ContainsKey(mType))
            {
                Debug.LogError($"Modules registry already contains {mType.Name} module");
                return;
            }

            if (config.safeMode)
            {
                try
                {
                    module.Init(this, mConfig);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Module {mType.Name} encountered initialization error: {ex.Message}. " +
                        $"<i><color=yellow>Error was captured in safemode. In order to get hard errors, disable safe mode in AppConfig</color></i>");
                }
            }
            else
            {
                module.Init(this, mConfig);
            }

            cachedModules.Add(mType, module);

            if (module is IProcessable)
            {
                processables.Add(module as IProcessable);
            }

            Debug.Log($"Registered {mType.Name} successfully");
        }

        public void CreateBehaviourModule<T>() where T : BehaviourModule
        {
            var moduleObject = new GameObject();
            moduleObject.transform.parent = transform;
            moduleObject.name = typeof(T).Name;
            T module = moduleObject.AddComponent<T>();

            RegisterModule(module);
        }

        public void Restart()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        public static void InitSignal(Action onInit)
        {
            if (Initialized)
            {
                onInit();
            }
            else
            {
                OnAppInitialized += onInit;
            }
        }
        private void Init(AppConfig config)
        {
            if (Core)
            {
                Debug.LogWarning($"{nameof(App)} already exists. " +
                    $"Make sure there's only one instance of the {nameof(App)}");

                if (this != Core)
                    Destroy(this);

                return;
            }

            Core = this;

            this.config = config;

            DontDestroyOnLoad(this);

            Debug.Log("Registering Modules...");

            CreateBehaviourModule<CoroutineManager>();
            RegisterModule(new Timer());

            if (config.modulesRegistry)
            {
                config.modulesRegistry.Init(this);
            }
            else
            {
                Debug.LogWarning($"{nameof(ModulesRegistry)} is null. Did you forget to assign it to {nameof(AppConfig)}?");
            }

            Debug.Log("Setting up scene manager...");

            void setupSceneManager(UnityEngine.SceneManagement.Scene scene)
            {
                var newSceneManager = FindObjectOfType<SceneManager>() as ISceneManager;

                if (newSceneManager != null)
                {
                    RegisterModule(newSceneManager);
                }
                else
                {
                    Debug.LogWarning($"{scene.name} doesn't have {nameof(SceneManager)}");
                }
            }

            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            setupSceneManager(activeScene);

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (oldScene, newScene) =>
            {
                if (newScene == activeScene)
                    return;

                Debug.Log($"Scene was changed to {newScene.name}");

                Type activeSceneManager = null;

                foreach (var module in cachedModules)
                {
                    if (module.Value is ISceneManager value)
                    {
                        activeSceneManager = module.Key;
                    }
                }

                if (activeSceneManager != null && cachedModules.ContainsKey(activeSceneManager))
                {
                    cachedModules.Remove(activeSceneManager);
                }

                setupSceneManager(newScene);
            };

            Initialized = true;

            OnAppInitialized?.Invoke();

            Debug.Log($"{nameof(App)} initialized");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BootUp()
        {
            if (Initialized)
                return;

            var config = Resources.Load<AppConfig>("AppConfig");

            if (config == null)
            {
                Debug.LogError($"App failed to boot up! Couldn't find {nameof(AppConfig)} in the Resources folder root");
                return;
            }

            var appObject = new GameObject(nameof(App)).AddComponent<App>();
            appObject.Init(config);
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
