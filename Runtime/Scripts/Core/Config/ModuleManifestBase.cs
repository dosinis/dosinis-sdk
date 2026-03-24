using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DosinisSDK.Core
{
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
        [NonSerialized] private Queue<(Task task, string description)> initializationQueue;

        internal async Task CreateUserModules(IModuleFactory moduleFactory)
        {
            this.moduleFactory = moduleFactory;
            initializationQueue = new Queue<(Task, string)>();
    
            CreateCoreModules(moduleFactory);
            BindUserModules();
    
            int total = initializationQueue.Count;
            int done = 0;

            foreach (var init in initializationQueue)
            {
                try
                {
                    await init.task;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                done++;

                float progress01 = total == 0 ? 1f : (float)done / total;
                int percent = Mathf.RoundToInt(progress01 * 80f);// modules accumulate to max 80% of app progress

                App.UpdateInitProgress(percent, init.description);
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
        protected abstract void BindUserModules();

        protected void BindCustomTask(Task task, string taskName = "")
        {
            initializationQueue.Enqueue((task, taskName));
        }
        
        protected void BindModule<TModule>(TModule source = null, ModuleConfig config = null)
            where TModule : class, IModule, new()
        {
            var currentTaskName = $"Loading {typeof(TModule).Name}...";
            initializationQueue.Enqueue((CreateModuleWithConfig(source, config), currentTaskName));
        }
        
        protected void BindModule<TModule>(AssetLink<ModuleConfig> configLink, TModule source = null)
            where TModule : class, IModule, new()
        {
            var currentTaskName = $"Loading {typeof(TModule).Name}...";
            initializationQueue.Enqueue((CreateModuleWithConfig(source, configLink), currentTaskName));
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