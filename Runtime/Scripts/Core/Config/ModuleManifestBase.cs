using System.Threading.Tasks;
using UnityEngine;

namespace DosinisSDK.Core
{
    /// <summary>
    /// NOTE: by default ModuleManifest registers modules before any SceneModule
    /// </summary>
    public class ModuleManifestBase : ScriptableObject
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

        internal async Task CreateUserModules(IModuleFactory moduleFactory)
        {
            await CreateModules(moduleFactory);
        }

        /// <summary>
        /// Add desired modules in override of this method
        /// </summary>
        protected virtual Task CreateModules(IModuleFactory moduleFactory)
        {
            moduleFactory.CreateModule<EventsManager>();
            moduleFactory.CreateModule<SceneManager>();
            moduleFactory.CreateModule<CoroutineManager>();
            moduleFactory.CreateModule<Timer>();
            moduleFactory.CreateModule<DataManager>(config: dataManagerConfig);
            moduleFactory.CreateModule<LocalClock>();
            return Task.CompletedTask;
        }
    }
}