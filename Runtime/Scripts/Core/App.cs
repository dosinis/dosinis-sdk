using System;
using System.Collections;
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

        private readonly Dictionary<Scene, List<SceneModule>> sceneModules = new();
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
        public static int InitProgress { get; private set; }
        public static bool Initialized { get; private set; }
        public static IApp Core { get; private set; }
        public const string MANIFEST_PATH = "ModuleManifest";
        
        // App
        
        public float CurrentFrameRate { get; private set; }
        
        public void Restart()
        {
            OnAppRestart?.Invoke();
            
            SceneManager.OnSceneUnloaded -= OnSceneUnloaded;
            SceneManager.OnSceneChanged -= OnSceneChanged;
            SceneManager.OnAdditiveSceneLoaded -= OnAdditiveSceneLoaded;
            
            var modulesToDispose = new List<IModule>(cachedModules.Values);

            foreach (var m in modulesToDispose)
            {
                if (m is BehaviourModule behaviourModule)
                {
                    DestroyImmediate(behaviourModule);
                }

                if (m is IDisposable disposableModule)
                {
                    disposableModule.Dispose();
                }
            }

            cachedModules.Clear();
            processables.Clear();
            fixedProcessables.Clear();
            lateProcessables.Clear();
            tickables.Clear();

            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0).completed += _ =>
            {
                Core = null;
                Init(manifest);
            };
        }

        public T GetModule<T>() where T : class, IModule
        {
            var mType = typeof(T);

            if (cachedModules.TryGetValue(mType, out var module))
                return (T)module;

            IModule found = null;

            foreach (var kv in cachedModules)
            {
                if (kv.Value is T)
                {
                    found = kv.Value;
                    break;
                }
            }

            if (found != null)
            {
                cachedModules[mType] = found;
                return (T)found;
            }

            Debug.LogError($"Module {typeof(T).Name} is not found! Maybe it's not ready yet?");
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

            if (cachedModules.TryGetValue(mType, out var m))
            {
                module = (T)m;
                return true;
            }

            IModule found = null;

            foreach (var kv in cachedModules)
            {
                if (kv.Value is T)
                {
                    found = kv.Value;
                    break;
                }
            }

            if (found != null)
            {
                cachedModules[mType] = found;
                module = (T)found;
                return true;
            }

            module = null;
            return false;
        }

        public bool IsModuleReady<T>() where T : class, IModule
        {
            var mType = typeof(T);

            if (cachedModules.ContainsKey(mType))
                return true;

            IModule found = null;

            foreach (var kv in cachedModules)
            {
                if (kv.Value is T)
                {
                    found = kv.Value;
                    break;
                }
            }

            if (found != null)
            {
                cachedModules[mType] = found;
                return true;
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
                if (manifest.SafeMode)
                {
                    try
                    {
                        module.Init(this, mConfig);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Module {mType.Name} encountered initialization error: {ex.Message}. \n {ex.StackTrace} \n" +
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
        
        public T CreateModule<T>(T source = null, ModuleConfig config = null) where T : class, IModule
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

        public async Task CreateModuleAsync<T>(T source = null, ModuleConfig config = null) where T : class, IAsyncModule
        {
            IAsyncModule module = source == null ? CreateModule<T>(config: config) : CreateModule(source, config);
            
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

        private void CleanupSceneModules(Scene scene)
        {
            var expiredSceneModules = new List<Type>();

            foreach (var sceneModule in sceneModules[scene])
            {
                var interfaces = sceneModule.GetType().GetInterfaces();
                
                foreach (var interfaceType in interfaces)
                {
                    // Handle duplicate cached modules, like IUIManager and UIManager
                    if (interfaceType != typeof(IModule) && typeof(IModule).IsAssignableFrom(interfaceType))
                    {
                        expiredSceneModules.Add(interfaceType);
                    }
                }
                
                expiredSceneModules.Add(sceneModule.GetType());
                    
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
                Debug.Log($"Removed {type.Name}!");
                cachedModules.Remove(type);
            }
        }
        
        private async Task SetupScene(Scene scene)
        {
            foreach (var rootObject in scene.GetRootGameObjects())
            {
                if (rootObject.activeSelf == false)
                    continue;

                if (sceneModules.ContainsKey(scene) == false)
                    sceneModules.Add(scene, new List<SceneModule>());
                
                sceneModules[scene].AddRange(rootObject.GetComponentsInChildren<SceneModule>());
            }
            
            sceneModules[scene].Sort((a, b) => b.InitPriority.CompareTo(a.InitPriority));

            IUIManager foundUIManager = null;

            var modules = new List<IModule>();
                
            foreach (var module in sceneModules[scene])
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

                if (manifest.SafeMode)
                {
                    try
                    {
                        if (module is IAsyncModule asyncModule)
                        {
                            await asyncModule.InitAsync(this);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Module {module.GetType().Name} encountered initialization error: {ex.Message}. \n {ex.StackTrace} \n" +
                                       $"<i><color=yellow>Error was captured in safemode. In order to get hard errors, disable safe mode in {MANIFEST_PATH}</color></i>");
                    }
                }
            }

            // Initializing UI as the last module for windows to be able to access other modules
            if (foundUIManager != null)
            {
                if (manifest.SafeMode)
                {
                    try
                    {
                        foundUIManager.Init(this, null);
                    
                        if (foundUIManager is IAsyncModule asyncModule)
                        {
                            await asyncModule.InitAsync(this);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Module {foundUIManager.GetType().Name} encountered initialization error: {ex.Message}. \n {ex.StackTrace} \n" +
                                       $"<i><color=yellow>Error was captured in safemode. In order to get hard errors, disable safe mode in {MANIFEST_PATH}</color></i>");
                    }
                }
                else
                {
                    foundUIManager.Init(this, null);
                    
                    if (foundUIManager is IAsyncModule asyncModule)
                    {
                        await asyncModule.InitAsync(this);
                    }
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

            InitProgress = 0;

            Core = this;

            this.manifest = manifest;

            if (manifest.TargetFramerate != 0)
                Application.targetFrameRate = manifest.TargetFramerate;
            
            if (manifest.PrewarmShaders)
                Shader.WarmupAllShaders();

            DontDestroyOnLoad(this);

            Debug.Log("Registering Modules...");

            InitProgress = 25;
            
            await manifest.CreateUserModules(this);

            InitProgress = 50;
            
            Debug.Log("Setting up scene modules...");
            
#if UNITY_WEBGL
            StartCoroutine(SkipFrameNative(continueScenesInit)); // Tiny delay for scene to be completely loaded (next frame)
#else
            await Task.Delay(1);
            _ = continueScenesInit();
#endif
            async Task continueScenesInit()
            {
                await SetupScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            
                SceneManager.OnSceneUnloaded += OnSceneUnloaded;
                SceneManager.OnSceneChanged += OnSceneChanged;
                SceneManager.OnAdditiveSceneLoaded += OnAdditiveSceneLoaded;

                InitProgress = 90;
                Initialized = true;
                
                onAppInitialized?.Invoke();

                Debug.Log($"{nameof(App)} initialized");
            }
        }

        public static void MarkAppBootupComplete()
        {
            InitProgress = 100;
        }

        private void OnSceneUnloaded(Scene scene)
        {
            Debug.Log($"Scene {scene.name} was unloaded");
            CleanupSceneModules(scene);
        }

        private async void OnAdditiveSceneLoaded(Scene scene)
        {
            Debug.Log($"Additive scene {scene.name} was loaded");
            await SetupScene(scene);
            Debug.Log($"Additive scene {scene.name} was initialized");
        }

        private IEnumerator SkipFrameNative(Action done)
        {
            yield return new WaitForEndOfFrame();
            
            done?.Invoke();
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
                if (manifest.SafeMode)
                {
                    try
                    {
                        processable.Process(Time.deltaTime);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error while processing {processable.GetType().Name}: {ex.Message}, {ex.StackTrace}");
                    }
                }
                else
                {
                    processable.Process(Time.deltaTime);
                }
            }
            
            if (Time.time - lastTick >= 1)
            {
                lastTick = Time.time;
                
                foreach (var tickable in tickables)
                {
                    try
                    {
                        tickable.Tick();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error while ticking {tickable.GetType().Name}: {ex.Message}, {ex.StackTrace}");
                    }
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
                if (manifest.SafeMode)
                {
                    try
                    {
                        fp.FixedProcess(Time.fixedDeltaTime);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error while fixed processing {fp.GetType().Name}: {ex.Message}, {ex.StackTrace}");
                    }
                }
                else
                {
                    fp.FixedProcess(Time.fixedDeltaTime);
                }
            }
        }
        
        private void LateUpdate()
        {
            foreach (var lp in lateProcessables)
            {
                if (manifest.SafeMode)
                {
                    try
                    {
                        lp.LateProcess(Time.deltaTime);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(
                            $"Error while late processing {lp.GetType().Name}: {ex.Message}, {ex.StackTrace}");
                    }
                }
                else
                {
                    lp.LateProcess(Time.deltaTime);
                }
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
