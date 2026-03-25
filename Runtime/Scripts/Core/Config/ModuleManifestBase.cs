using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class InitHandle
    {
        public Func<Task> task;
        public string description;
        public float weight;
    }
    
    /// <summary>
    /// NOTE: by default ModuleManifest registers modules before any SceneModule
    /// </summary>
    public abstract class ModuleManifestBase : ScriptableObject
    {
        [Header("AppConfig")] 
        [SerializeField] private bool prewarmShaders;
        [SerializeField] private int targetFramerate = 60;

        [Header("ModulesManifest")]
        [SerializeField] private bool safeMode = true;
        [SerializeField] private DataManagerConfig dataManagerConfig;
        
        internal bool PrewarmShaders => prewarmShaders;
        internal int TargetFramerate => targetFramerate;
        internal bool SafeMode => safeMode;

        [NonSerialized] private GlobalAssetsManager assetManager;
        [NonSerialized] private IModuleFactory moduleFactory;
        [NonSerialized] private Queue<InitHandle> initializationQueue;

        internal async Task CreateUserModules(IModuleFactory moduleFactory)
        {
            this.moduleFactory = moduleFactory;
            initializationQueue = new Queue<InitHandle>();
            
            CreateCoreModules(moduleFactory);
            
            App.UpdateInitProgress(10, "Binding modules...");
            await BindUserModules();
            await RunInitializationQueue();
        }

        private async Task RunInitializationQueue()
        {
            float totalWeight = 0f;
            
            foreach (var s in initializationQueue)
                totalWeight += s.weight;

            float doneWeight = 0f;

            foreach (var init in initializationQueue)
            {
                App.UpdateInitStatus(init.description);

                try
                {
                    await init.task.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                
                doneWeight += init.weight;
                
                float p01 = totalWeight <= 0 ? 1f : doneWeight / totalWeight;
                int percent = 20 + Mathf.RoundToInt(p01 * 60f);// 20-80% for modules
                App.UpdateInitProgress(Mathf.RoundToInt(percent), init.description);
            }
        }

        protected virtual void CreateCoreModules(IModuleFactory moduleFactory)
        {
            moduleFactory.CreateModule<EventsManager>();
            moduleFactory.CreateModule<SceneManager>();
            moduleFactory.CreateModule<CoroutineManager>();
            moduleFactory.CreateModule<Timer>();
            moduleFactory.CreateModule<DataManager>(config: dataManagerConfig);
            moduleFactory.CreateModule<LocalClock>();
            assetManager = moduleFactory.CreateModule<GlobalAssetsManager>();
            SetupAssetProvider();
        }
        
        protected virtual void SetupAssetProvider()
        {
            assetManager.SetProvider(new AssetProviderResources());
        }

        /// <summary>
        /// Add desired modules in override of this method
        /// </summary>
        protected abstract Task BindUserModules();

        protected void BindCustomTask(Func<Task> task, string taskName = "", float weight = 1f)
        {
            var initHandle = new InitHandle
            {
                task = task,
                description = taskName,
                weight = weight
            };
            
            initializationQueue.Enqueue(initHandle);
        }
        
        protected void BindModule<TModule>(TModule source = null, ModuleConfig config = null, float weight = 1f)
            where TModule : class, IModule, new()
        {
            var currentTaskName = $"Loading {typeof(TModule).Name}...";
            
            var initHandle = new InitHandle
            {
                task = () => CreateModuleWithConfig(source, config),
                description = currentTaskName,
                weight = weight,
            };
            
            initializationQueue.Enqueue(initHandle);
        }
        
        protected void BindModule<TModule>(AssetLink<ModuleConfig> configLink, TModule source = null, float weight = 1f)
            where TModule : class, IModule, new()
        {
            var currentTaskName = $"Loading {typeof(TModule).Name}...";
            
            var initHandle = new InitHandle
            {
                task = () => CreateModuleWithConfig(source, configLink),
                description = currentTaskName,
                weight = weight,
            };
            
            initializationQueue.Enqueue(initHandle);
        }

        private async Task CreateModuleWithConfig<TModule>(TModule source, ModuleConfig config)
            where TModule : class, IModule, new()
        {
            if (source == null)
            {
                moduleFactory.CreateModule<TModule>(config: config);
            }
            else
            {
                if (source is IAsyncModule asyncModule)
                {
                    await moduleFactory.CreateModuleAsync(asyncModule, config: config);
                }
                else
                {
                    moduleFactory.CreateModule(source, config);
                }
            }
        }
        
        private async Task CreateModuleWithConfig<TModule>(TModule source, AssetLink<ModuleConfig> configLink)
            where TModule : class, IModule, new()
        {
            var config = await configLink.GetAssetAsync();
            await CreateModuleWithConfig(source, config);
        }
    }
}