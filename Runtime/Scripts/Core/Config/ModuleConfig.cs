using UnityEngine;

namespace DosinisSDK.Core
{
    [CreateAssetMenu(fileName = "ModuleConfig", menuName = "DosinisSDK/Configs/ModuleConfig")]
    public class ModuleConfig : ScriptableObject
    {
        public bool enableLogs = true;
    }
}
