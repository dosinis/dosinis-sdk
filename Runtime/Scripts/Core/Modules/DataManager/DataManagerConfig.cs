using UnityEngine;

namespace DosinisSDK.Core
{
    [CreateAssetMenu(menuName = "DosinisSDK/Core/DataManagerConfig", fileName = "DataManagerConfig")]
    public class DataManagerConfig : ModuleConfig
    {
        [SerializeField] private int wipeVersion = 0;

        public int WipeVersion => wipeVersion;
    }
}
