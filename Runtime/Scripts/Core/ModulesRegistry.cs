using UnityEngine;

namespace DosinisSDK.Core
{
    /// <summary>
    /// NOTE: by default ModulesRegistry registers modules before SceneManager or UIManager
    /// </summary>
    public class ModulesRegistry : ScriptableObject
    {
        internal void Init(IApp app)
        {
            OnInit(app);
        }

        /// <summary>
        /// Add desired modules in override of this method.
        /// NOTE: You may also pass Module config argument in app.RegisterModule(new Module(), moduleConfig);
        /// </summary>
        /// <param name="app"></param>
        protected virtual void OnInit(IApp app)
        {
            app.RegisterModule(new DataManager());

            // Behaviour module creation example:

            // app.CreateBehaviourModule<BehaviourModule>();
        }
    }
}