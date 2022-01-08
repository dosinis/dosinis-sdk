using UnityEngine;

namespace DosinisSDK.Core
{
    /// NOTE: by default ModulesRegistry registers modules before SceneManager or UIManager
    public class ModulesRegistry : MonoBehaviour // Should be ScriptableObject or even part of AppConfig? Name idea: ModulesBootstrapper?
    {
        /// <summary>
        /// Add desired modules in override of this method.
        /// NOTE: You may also pass Module config argument in app.RegisterModule(new Module(), moduleConfig);
        /// </summary>
        /// <param name="app"></param>
        public virtual void Init(IApp app)
        {
            app.RegisterModule(new DataManager());

            // Behaviour module creation example:

            // app.CreateBehaviourModule<BehaviourModule>();
        }
    }
}
