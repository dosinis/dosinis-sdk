using System.Threading.Tasks;
using UnityEngine;

namespace DosinisSDK.Core
{
    /// <summary>
    /// NOTE: by default ModuleManifest registers modules before any SceneModule
    /// </summary>
    public class ModuleManifestBase : ScriptableObject
    {
        public bool prewarmShaders;
        public bool safeMode = true;

        internal async Task CreateUserModules(IModuleFactory moduleFactory)
        {
            await CreateModules(moduleFactory);
        }

        /// <summary>
        /// Add desired modules in override of this method
        /// </summary>
        protected virtual Task CreateModules(IModuleFactory moduleFactory)
        {
            moduleFactory.CreateModule<SceneManager>();
            moduleFactory.CreateModule<CoroutineManager>();
            moduleFactory.CreateModule<Timer>();
            moduleFactory.CreateModule<DataManager>();
            moduleFactory.CreateModule<LocalClock>();
            return Task.CompletedTask;
        }
    }
}
