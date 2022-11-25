using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DosinisSDK.Core
{
    public sealed class App : MonoBehaviour, IApp
    {
        [SerializeField] private bool prewarmShaders;
        
        // Private

        private readonly Dictionary<Type, IModule> cachedModules = new Dictionary<Type, IModule>();
        private readonly List<IProcessable> processables = new List<IProcessable>();

        private AppConfigBase config;
        private AsyncOperation loadSceneOperation;
        
        // Events

        public event Action<bool> OnAppPaused;
        public event Action<bool> OnAppFocus;
        public event Action OnAppQuit;
        public event Action OnAppRestart;

        // Core Modules

        public static CoroutineManager Coroutine => Core.GetModule<CoroutineManager>();
        public static IDataManager Data => Core.GetModule<IDataManager>();
        public static IClock Clock => Core.GetModule<IClock>();
        public static ITimer Timer => Core.GetModule<ITimer>();
        public static ISceneManager SceneManager => Core.GetModule<ISceneManager>();
        public static UIManager UIManager => Core.GetModule<UIManager>();

        // Properties

        public float SceneLoadProgress { get; private set; }

        // Static

        private static Action onAppInitialized;
        public static bool Initialized { get; private set; }
        public static App Core { get; private set; }
        public const string CONFIG_PATH = "AppConfig";

        // App
        
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
        
        public bool TryGetModule<T>(out T module) where T : class, IModule
        {
            var mType = typeof(T);

            if (cachedModules.TryGetValue(mType, out IModule m))
            {
                module = (T)m;
                return true;
            }

            foreach (var mKeyValue in cachedModules)
            {
                if (mKeyValue.Value is T value)
                {
                    cachedModules.Add(mType, mKeyValue.Value);
                    module = value;
                    return true;
                }
            }
            
            module = null;
            return false;
        }

        public bool IsModuleReady<T>() where T : class, IModule
        {
            var mType = typeof(T);

            if (cachedModules.ContainsKey(mType))
            {
                return true;
            }

            foreach (var mKeyValue in cachedModules)
            {
                if (mKeyValue.Value is T)
                {
                    cachedModules.Add(mType, mKeyValue.Value);
                    return true;
                }
            }
            
            return false;
        }

        private void RegisterModule(IModule module, ModuleConfig mConfig = null)
        {
            var mType = module.GetType();

            if (cachedModules.ContainsKey(mType))
            {
                Debug.LogError($"Modules registry already contains {mType.Name} module");
                return;
            }

            cachedModules.Add(mType, module);
            
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
            
            if (module is IProcessable processabe)
            {
                if (processables.Contains(processabe)== false)
                {
                    processables.Add(processabe);
                }
            }

            Debug.Log($"Registered {mType.Name} successfully");
        }
        
        public T CreateModule<T>(T source = default, ModuleConfig config = null) where T : class, IModule
        {
            if (typeof(T).IsSubclassOf(typeof(BehaviourModule)))
            {
                if (source == null)
                {  
                    var moduleObject = new GameObject();
                    moduleObject.transform.parent = transform;
                    moduleObject.name = typeof(T).Name;
                    var module = moduleObject.AddComponent(typeof(T)) as IModule;

                    RegisterModule(module, config);

                    return module as T;
                }
                else
                {   
                    var moduleObj = Instantiate(source as BehaviourModule, transform);

                    if (moduleObj is T module)
                    {
                        RegisterModule(module, config);
                        return module;
                    }

                    Debug.LogError($"{moduleObj.name} doesn't contain {typeof(T).Name} component attached");
                    return default;
                }
            }

            if (source == null)
            {
                var module = (IModule)Activator.CreateInstance(typeof(T));
                RegisterModule(module, config);
                return module as T;
            }

            RegisterModule(source, config);
            return source;
        }

        public async Task CreateModuleAsync<T>(T source = default, ModuleConfig config = null) where T : class, IModule, IAsyncModule
        {
            var module = CreateModule<T>(source, config) as IAsyncModule;

            await module.InitAsync(this, config);
        }

        public void Restart()
        {
            OnAppRestart?.Invoke();
            LoadScene(0);
        }

        public void SwitchLoadedScene()
        {
            loadSceneOperation.allowSceneActivation = true;
        }

        public void LoadScene(int sceneIndex, LoadSceneMode mode = LoadSceneMode.Single, bool switchLoadedScene = true, Action done = null)
        {
            StartCoroutine(LoadSceneCoroutine(sceneIndex, mode, switchLoadedScene, done));
        }

        private IEnumerator LoadSceneCoroutine(int sceneIndex, LoadSceneMode mode, bool switchLoadedScene, Action done)
        {
            SceneLoadProgress = 0;

            yield return new WaitForSeconds(0.5f);

            loadSceneOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, mode);

            loadSceneOperation.allowSceneActivation = switchLoadedScene;
            
            while (!loadSceneOperation.isDone)
            {
                yield return null;
                SceneLoadProgress = loadSceneOperation.progress;
            }

            SceneLoadProgress = 1;
            
            yield return new WaitForSeconds(0.1f);
            
            done?.Invoke();
        }

        public static void Ready(Action onReady)
        {
            if (Initialized)
            {
                onReady();
            }
            else
            {
                onAppInitialized += onReady;
            }
        }

        public static async Task Ready()
        {
            while (Initialized == false)
            {
                await Task.Yield();
            }
        }

        private void SetSceneChangedCallback(Action<Scene> onSceneChanged)
        {
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

                    if (cache.Value is UIManager uiManager)
                    {
                        duplicateModules.Add(uiManager);
                    }
                }

                var modulesToRemove = new List<Type>();

                foreach (var module in duplicateModules)
                {
                    foreach (var cache in cachedModules)
                    {
                        if (cache.Value != module) 
                            continue;
                        
                        modulesToRemove.Add(cache.Key);

                        if (module is IProcessable processable)
                        {
                            if (processables.Contains(processable))
                                processables.Remove(processable);
                        }
                    }
                }      
                
                foreach (var module in modulesToRemove)
                {
                    cachedModules.Remove(module);
                }
                
                onSceneChanged?.Invoke(newScene);
            };
        }
        
        private async void Init(AppConfigBase config)
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

            await config.CreateUserModules(this);

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

            await Task.Delay(1); // Tiny delay for scene to be completely loaded (next frame)
            
            setupScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            
            SetSceneChangedCallback(setupScene);

            Initialized = true;

            onAppInitialized?.Invoke();

            Debug.Log($"{nameof(App)} initialized");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BootUp()
        {
            if (Initialized)
                return;

            var config = Resources.Load<AppConfigBase>(CONFIG_PATH);

            if (config == null)
            {
                throw new FileNotFoundException($"App failed to boot up! Couldn't find {CONFIG_PATH} in the Resources folder");
            }

            var appObject = new GameObject(nameof(App)).AddComponent<App>();
            appObject.Init(config);
        }

        // MonoBehaviour
        
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
