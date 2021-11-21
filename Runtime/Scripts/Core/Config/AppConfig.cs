using UnityEngine;

namespace DosinisSDK.Core
{
    [CreateAssetMenu(fileName = "ModuleConfig", menuName = "DosinisSDK/Configs/AppConfig")]
    public class AppConfig : ScriptableObject
    {
        public BehaviourModule[] behaviourModules;
    }
}
