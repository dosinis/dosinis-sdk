using UnityEngine;

namespace DosinisSDK.Core
{
    public class ModulesRegistry : MonoBehaviour
    {
        /// <summary>
        /// Add desired non-behaviour modules in override of this method.
        /// You may also pass Module config argument in app.RegisterModule(new Module(), moduleConfig);
        /// </summary>
        /// <param name="app"></param>
        public virtual void Init(IApp app)
        {
            app.RegisterModule(new DataManager());
        }
    }
}
