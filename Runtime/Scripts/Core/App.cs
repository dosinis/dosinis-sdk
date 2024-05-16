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

        private readonly Dictionary<Type, IModule> cachedModules = new();
        private readonly HashSet<IProcessable> processables = new();
        private readonly HashSet<ITickable> tickables = new();
        private readonly HashSet<IFixedProcessable> fixedProcessables = new();
        private readonly HashSet<ILateProcessable> lateProcessables = new();

        private ModuleManifestBase manifest;
        private float lastTick;
        private int framesPassed;
        private float nextFrame;
        private const float FPS_MEASURE_PERIOD = 0.5f;
        
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
        
        public float CurrentFrameRate { get; private set; }
        
        public void Restart()
        {
            OnAppRestart?.Invoke();
            
            SceneManager.OnSceneAboutToChange -= CleanupSceneModules;
            SceneManager.OnSceneChanged -= OnSceneChanged;
            
            foreach (var module in cachedModules)
            {
                if (module.Value is BehaviourModule behaviourModule)
                {
                    DestroyImmediate(behaviourModule);
                }
                
                if (module.Value is IDisposable disposableModule)
                {
                    disposableModule.Dispose();
                }
            }
            
            cachedModules.Clear();
            processables.Clear();
            fixedProcessables.Clear();
            lateProcessables.Clear();
            tickables.Clear();

            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            
            Core = null;

            Init(manifest);
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

        private void RegisterModule(IModule module, ModuleConfig mConfig = null, bool init = true)
        {
            var mType = module.GetType();

            if (cachedModules.ContainsKey(mType))
            {
                Debug.LogError($"Modules registry already contains {mType.Name} module");
                return;
            }

            cachedModules.Add(mType, module);

            if (init)
            {
                if (manifest.safeMode)
                {
                    try
                    {
                        module.Init(this, mConfig);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Module {mType.Name} encountered initialization error: {ex.Message}. " +
                                       $"<i><color=yellow>Error was captured in safemode. In order to get hard errors, disable safe mode in {MANIFEST_PATH}</color></i>");
                    }
                }
                else
                {
                    module.Init(this, mConfig);
                }
            }
            
            if (module is IProcessable processable)
            {
                processables.Add(processable);
            }
            
            if (module is IFixedProcessable fixedProcessable)
            {
                fixedProcessables.Add(fixedProcessable);
            }

            if (module is ILateProcessable lateProcessable)
            {
                lateProcessables.Add(lateProcessable);
            }
            
            if (module is ITickable tickable)
            {
                tickables.Add(tickable);
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
            var expiredSceneModules = new List<Type>();

            foreach (var cache in cachedModules)
            {
                if (cache.Value is not SceneModule sceneModule) 
                    continue;
                
                expiredSceneModules.Add(cache.Key);
                    
                if (sceneModule is IProcessable processable)
                {
                    processables.Remove(processable);
                }
                
                if (sceneModule is IFixedProcessable fixedProcessable)
                {
                    fixedProcessables.Remove(fixedProcessable);
                }
                
                if (sceneModule is ILateProcessable lateProcessable)
                {
                    lateProcessables.Remove(lateProcessable);
                }
                
                if (sceneModule is ITickable tickable)
                {
                    tickables.Remove(tickable);
                }
            }

            foreach (var type in expiredSceneModules)
            {
                cachedModules.Remove(type);
            }
        }
        
        private async Task SetupScene(Scene scene)
        {
            var sceneModules = FindObjectsOfType<SceneModule>();
                
            Array.Sort(sceneModules, (a, b) => b.InitPriority.CompareTo(a.InitPriority));

            IUIManager foundUIManager = null;

            var modules = new List<IModule>();
                
            foreach (var module in sceneModules)
            {
                if (module is IUIManager uiManager)
                {
                    foundUIManager = uiManager;
                }
                else
                {
                    modules.Add(module);
                }
            }

            // Registering UIManager as the very first scene module for other scene modules to be able to cache it on Init
            if (foundUIManager != null)
            {
                RegisterModule(foundUIManager, init: false);
            }
            else
            {
                Debug.LogWarning($"{scene.name} doesn't have {nameof(UIManager)}. Ignore if it's intended");
            }

            foreach (var module in modules)
            {
                RegisterModule(module);

                if (module is IAsyncModule asyncModule)
                {
                    await asyncModule.InitAsync(this);
                }
            }

            // Initializing UI as the last module for windows to be able to access other modules
            if (foundUIManager != null)
            {
                foundUIManager.Init(this, null);
                    
                if (foundUIManager is IAsyncModule asyncModule)
                {
                    await asyncModule.InitAsync(this);
                }
            }
        }
        
        private async void Init(ModuleManifestBase manifest)
        {
            if (Core != null)
            {
                Debug.LogWarning($"{nameof(App)} already exists. " +
                                 $"Make sure there's only one instance of the {nameof(App)}");

                if (this != (App)Core)
                    DestroyImmediate(gameObject);

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

            await Task.Delay(1); // Tiny delay for scene to be completely loaded (next frame)
            
            await SetupScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            
            SceneManager.OnSceneAboutToChange += CleanupSceneModules;
            SceneManager.OnSceneChanged += OnSceneChanged;

            Initialized = true;

            onAppInitialized?.Invoke();

            Debug.Log($"{nameof(App)} initialized");
        }

        private async void OnSceneChanged((Scene oldScene, Scene newScene) args)
        {
            Debug.Log($"Scene was changed into {args.newScene.name}");
            await SetupScene(args.newScene);
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
            
            if (Time.time - lastTick >= 1)
            {
                lastTick = Time.time;
                
                foreach (var tickable in tickables)
                {
                    tickable.Tick();
                }
            }
            
            framesPassed++;
            
            if (Time.realtimeSinceStartup > nextFrame)
            {
                CurrentFrameRate = (int)(framesPassed / FPS_MEASURE_PERIOD);
                framesPassed = 0;
                nextFrame += FPS_MEASURE_PERIOD;
            }
        }

        private void FixedUpdate()
        {
            foreach (var fp in fixedProcessables)
            {
                fp.FixedProcess(Time.fixedDeltaTime);
            }
        }
        
        private void LateUpdate()
        {
            foreach (var lp in lateProcessables)
            {
                lp.LateProcess(Time.deltaTime);
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
