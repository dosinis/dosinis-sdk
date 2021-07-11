using UnityEngine;

namespace DosinisSDK.Core
{
    public class ModulesRegistry : MonoBehaviour
    {
        public virtual void Init(IApp app)
        {
            app.RegisterModule(new DataManager());
        }
    }
}

