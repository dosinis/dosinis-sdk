using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DosinisSDK.Core
{
    public sealed class App : MonoBehaviour, IApp, IModuleFactory
    {
        // Private

        private readonly Dictionary<Type, IModule> cachedModules = new Dictionary<Type, IModule>();
        private readonly List<IProcessable> processables = new List<IProcessable>();

        private ModuleManifestBase manifest;

        // Events

        public event Action<bool> OnAppPaused;
        public event Action<bool> OnAppFocus;
        public event Action OnAppQuit;
        public event Action OnAppRestart;

        // Core Modules
        
        public ICoroutineManager Coroutine => GetModule<ICoroutineManager>();
        public IDataManager DataManager => GetModule<IDataManager>();
        public IClock Clock => GetModule<IClock>();
        public ITimer Timer => GetModule<ITimer>();
        public ISceneManager SceneManager => GetModule<ISceneManager>();
        public IUIManager UIManager => GetModule<IUIManager>();
        
        // Static

        private static Action onAppInitialized;
        public static bool Initialized { get; private set; }
        public static IApp Core { get; private set; }
        public const string MANIFEST_PATH = "ModuleManifest";

        // App
        
        public void Restart()
        {
            OnAppRestart?.Invoke();
            SceneManager.LoadScene(0);
        }
        
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

        public async Task<T> WaitForModule<T>(float? timeOut = null) where T : class, IModule
        {
            float startTime = Time.time;
            
            while (IsModuleReady<T>() == false)
            {
                if (timeOut != null && Time.time - startTime > timeOut)
                {
                    Debug.LogError($"Module is not ready after {timeOut} seconds");
                    return default;
                }
                
                await Task.Yield();
            }
            
            return GetModule<T>();
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
            
            if (manifest.safeMode)
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
                if (processables.Contains(processabe) == false)
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

        public async Task CreateModuleAsync<T>(T source = default, ModuleConfig config = null) where T : class, IAsyncModule
        {
            var module = CreateModule(source, config) as IAsyncModule;

            await module.InitAsync(this);
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

        private void CleanupSceneModules()
        {
            var duplicateModules = new List<IModule>();

            foreach (var cache in cachedModules)
            {
                if (cache.Value is not SceneModule sceneModule) 
                    continue;
                
                duplicateModules.Add(sceneModule);
                    
                if (sceneModule is IProcessable processable)
                {
                    if (processables.Contains(processable))
                        processables.Remove(processable);
                }
            }

            foreach (var module in duplicateModules)
            {
                cachedModules.Remove(module.GetType());
            }
        }
        
        private async void Init(ModuleManifestBase manifest)
        {
            if (Core != null)
            {
                Debug.LogWarning($"{nameof(App)} already exists. " +
                                 $"Make sure there's only one instance of the {nameof(App)}");

                if (this != (App)Core)
                    Destroy(this);

                return;
            }

            Debug.Log("Starting App...");

            Core = this;

            this.manifest = manifest;

            if (manifest.targetFramerate != 0)
                Application.targetFrameRate = manifest.targetFramerate;
            
            if (manifest.prewarmShaders)
                Shader.WarmupAllShaders();

            DontDestroyOnLoad(this);

            Debug.Log("Registering Modules...");

            await manifest.CreateUserModules(this);

            Debug.Log("Setting up scene modules...");

            void setupScene(Scene scene)
            {
                var sceneModules = FindObjectsOfType<SceneModule>();

                IUIManager foundUIManager = null;
                
                foreach (var module in sceneModules)
                {
                    if (module is IUIManager uiManager)
                    {
                        foundUIManager = uiManager;
                    }
                    else
                    {
                        RegisterModule(module);
                    }
                }

                // Registering UIManager as the very last module
                if (foundUIManager != null)
                {
                    RegisterModule(foundUIManager);
                }
                else
                {
                    Debug.LogWarning($"{scene.name} doesn't have {nameof(UIManager)}. Ignore if it's intended");
                }
            }

            await Task.Delay(1); // Tiny delay for scene to be completely loaded (next frame)
            
            setupScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            
            SceneManager.OnSceneChanged += (oldScene, newScene) =>
            {
                Debug.Log($"Scene was changed into {newScene.name}");
                CleanupSceneModules();
                setupScene(newScene);
            };

            Initialized = true;

            onAppInitialized?.Invoke();

            Debug.Log($"{nameof(App)} initialized");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BootUp()
        {
            if (Initialized)
                return;

            var manifest = Resources.Load<ModuleManifestBase>(MANIFEST_PATH);

            if (manifest == null)
            {
                throw new FileNotFoundException($"App failed to boot up! Couldn't find {MANIFEST_PATH} in the Resources folder");
            }

            var appObject = new GameObject(nameof(App)).AddComponent<App>();
            appObject.Init(manifest);
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
