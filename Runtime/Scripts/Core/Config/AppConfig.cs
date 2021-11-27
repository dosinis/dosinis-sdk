using UnityEngine;

namespace DosinisSDK.Core
{
    [CreateAssetMenu(fileName = "AppConfig", menuName = "DosinisSDK/Configs/AppConfig")]
    public sealed class AppConfig : ScriptableObject
    {
        public bool safeMode = true;
        public BehaviourModule[] behaviourModules;
        public ModulesRegistry modulesRegistry;
    }
}
