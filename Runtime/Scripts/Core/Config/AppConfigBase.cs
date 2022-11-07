using System.Threading.Tasks;
using UnityEngine;

namespace DosinisSDK.Core
{
    /// <summary>
    /// NOTE: by default AppConfig registers modules before SceneManager or UIManager
    /// </summary>
    public class AppConfigBase : ScriptableObject
    {
        public bool safeMode = true;

        internal async Task CreateUserModules(IApp app)
        {
            await CreateModules(app);
        }

        /// <summary>
        /// Add desired modules in override of this method
        /// </summary>
        /// <param name="app"></param>
        protected virtual Task CreateModules(IApp app)
        {
            app.CreateModule<CoroutineManager>();
            app.CreateModule<Timer>();
            app.CreateModule<DataManager>();
            app.CreateModule<LocalClock>();
            return Task.CompletedTask;
        }
    }
}
