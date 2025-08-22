using UnityEngine;

namespace DosinisSDK.Core
{
    [CreateAssetMenu(menuName = "DosinisSDK/Core/DataManagerConfig", fileName = "DataManagerConfig")]
    public class DataManagerConfig : ModuleConfig
    {
        [SerializeField] private int wipeVersion = 0;
        [SerializeField] private bool wipeAllPrefs = true;
        [SerializeField] private bool forceWipeOnStartup = false;
        
        public bool ForceWipeOnStartup => forceWipeOnStartup;
        public int WipeVersion => wipeVersion;
        public bool WipeAllPrefs => wipeAllPrefs;
    }
}