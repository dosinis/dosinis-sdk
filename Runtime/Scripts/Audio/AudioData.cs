using DosinisSDK.Core;

namespace DosinisSDK.Audio
{
    [System.Serializable]
    public class AudioData : ModuleData
    {
        public float volume;
        public bool isSfxMuted;
        public bool isMusicMuted;
    }
}
