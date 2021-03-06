using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DosinisSDK.Core
{
    public sealed class App : MonoBehaviour, IApp
    {
        [SerializeField] private bool prewarmShaders = false;
        
        // Private

        private readonly Dictionary<Type, IModule> cachedModules = new Dictionary<Type, IModule>();
        private readonly List<IProcessable> processables = new List<IProcessable>();

        private AppConfig config;

        // Events

        public event Action<bool> OnAppPaused;
        public event Action<bool> OnAppFocus;
        public event Action OnAppQuit;

        // Core Modules

        public ITimer Timer => GetModule<ITimer>();
        public ICoroutineManager Coroutine => GetModule<ICoroutineManager>();
        public ISceneManager SceneManager => GetModule<ISceneManager>();
        public IUIManager UIManager => GetModule<IUIManager>();

        // Properties

        public float SceneLoadProgress { get; private set; }

        // Static

        public static bool Initialized { get; private set; }
        private static Action OnAppInitialized;

        public static App Core;

        public T GetModule<T>() where T : class, IModule
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

            if (module is IProcessable processabe)
            {
                if (processables.Contains(processabe)== false)
                {
                    processables.Add(processabe);
                }
            }

            Debug.Log($"Registered {mType.Name} successfully");
        }

        public void CreateBehaviourModule<T>(T source = null) where T : BehaviourModule
        {
            if (source == null)
            {
                var moduleObject = new GameObject();
                moduleObject.transform.parent = transform;
                moduleObject.name = typeof(T).Name;
                T module = moduleObject.AddComponent<T>();

                RegisterModule(module);
            }
            else
            {
                BehaviourModule moduleObj = Instantiate(source, transform);

                if (moduleObj is T module)
                {
                    RegisterModule(module);
                }
                else
                {
                    Debug.LogError($"{source.name} doesn't contain {typeof(T).Name} component attached");
                }
            }
        }

        public void Restart()
        {
            LoadScene(0);
        }

        public void LoadScene(int sceneIndex, Action done = null, LoadSceneMode mode = LoadSceneMode.Single)
        {
            StartCoroutine(LoadSceneCoroutine(sceneIndex, mode, done));
        }

        private IEnumerator LoadSceneCoroutine(int sceneIndex, LoadSceneMode mode, Action done)
        {
            SceneLoadProgress = 0;

            yield return new WaitForSeconds(0.5f);

            var loadSceneOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, mode);

            while (!loadSceneOperation.isDone)
            {
                yield return null;
                SceneLoadProgress = loadSceneOperation.progress;
            }

            yield return new WaitForSeconds(0.1f);

            SceneLoadProgress = 1;

            done?.Invoke();
        }

        public static void ModulesReady(Action onInit)
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

            Debug.Log("Starting App...");

            Core = this;

            this.config = config;

            if (prewarmShaders)
                Shader.WarmupAllShaders();

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

            void setupScene(Scene scene)
            {
                var newSceneManager = FindObjectOfType<SceneManager>();
                var newUIManager = FindObjectOfType<UIManager>();

                if (newSceneManager != null)
                {
                    RegisterModule(newSceneManager);
                }
                else
                {
                    Debug.LogWarning($"{scene.name} doesn't have {nameof(SceneManager)}. Ignore if it's intended");
                }

                if (newUIManager != null)
                {
                    RegisterModule(newUIManager);
                }
                else
                {
                    Debug.LogWarning($"{scene.name} doesn't have {nameof(UIManager)}. Ignore if it's intended");
                }
            }

            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (oldScene, newScene) =>
            {
                Debug.Log($"Scene was changed into {newScene.name}");

                var duplicateModules = new List<IModule>();

                foreach (var cache in cachedModules)
                {
                    if (cache.Value is ISceneManager sceneManager)
                    {
                        duplicateModules.Add(sceneManager);
                    }

                    if (cache.Value is IUIManager uiManager)
                    {
                        duplicateModules.Add(uiManager);
                    }
                }

                var modulesToRemove = new List<Type>();

                foreach (var module in duplicateModules)
                {
                    foreach (var cache in cachedModules)
                    {
                        if (cache.Value == module)
                        {
                            modulesToRemove.Add(cache.Key);

                            if (module is IProcessable processable)
                            {
                                if (processables.Contains(processable))
                                    processables.Remove(processable);
                            }
                        }
                    }
                }      
                
                foreach (var module in modulesToRemove)
                {
                    cachedModules.Remove(module);
                }

                setupScene(newScene);
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
