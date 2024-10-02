using UnityEngine;

namespace DosinisSDK.Core
{
    [CreateAssetMenu(menuName = "DosinisSDK/Core/DataManagerConfig", fileName = "DataManagerConfig")]
    public class DataManagerConfig : ModuleConfig
    {
        [SerializeField] private int wipeVersion = 0;
        [SerializeField] private bool wipeAllPrefs = true;

        public int WipeVersion => wipeVersion;
        public bool WipeAllPrefs => wipeAllPrefs;
    }
}
