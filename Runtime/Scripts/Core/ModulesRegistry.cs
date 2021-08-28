using UnityEngine;

namespace DosinisSDK.Core
{
    public class ModulesRegistry : MonoBehaviour
    {
        [SerializeField] private ModuleConfig[] configs;

        public ModuleConfig[] Configs => configs;

        public virtual void Init(IApp app)
        {
            app.RegisterModule(new DataManager());
        }
    }
}

