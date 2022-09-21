using UnityEngine;

namespace DosinisSDK.Core
{
    /// <summary>
    /// NOTE: by default AppConfig registers modules before SceneManager or UIManager
    /// </summary>
    public class AppConfigBase : ScriptableObject
    {
        public bool safeMode = true;

        internal void CreateUserModules(IApp app)
        {
            CreateModules(app);
        }

        /// <summary>
        /// Add desired modules in override of this method.
        /// NOTE: You may also pass Module config argument in app.RegisterModule(new Module(), moduleConfig);
        /// </summary>
        /// <param name="app"></param>
        protected virtual void CreateModules(IApp app)
        {
            app.RegisterModule(new DataManager());

            // Behaviour module creation example:
            // app.CreateBehaviourModule<BehaviourModule>();
        }
    }
}
