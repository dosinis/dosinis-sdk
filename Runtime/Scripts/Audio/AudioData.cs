using DosinisSDK.Core;

namespace DosinisSDK.Audio
{
    [System.Serializable]
    public class AudioData : ModuleData
    {
        public float volume = 1;
        public bool isSfxEnabled = true;
        public bool isMusicEnabled = true;
    }
}
