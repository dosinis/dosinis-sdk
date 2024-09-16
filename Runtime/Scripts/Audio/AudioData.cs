using DosinisSDK.Core;

namespace DosinisSDK.Audio
{
    [System.Serializable]
    public class AudioData : ModuleData
    {
        public float masterVolume = 1f;
        public float musicVolume = 1f;
        public float soundsVolume = 1f;
        public bool isSfxEnabled = true;
        public bool isMusicEnabled = true;
    }
}
