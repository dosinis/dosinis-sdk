using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Audio
{
    [CreateAssetMenu(menuName = "Tattoo/AudioConfig", fileName = "AudioConfig")]
    
    public class AudioConfig: ModuleConfig
    {
        [SerializeField] private float masterVolume = 1f;
        [SerializeField] private float musicVolume = 1f;
        [SerializeField] private float sfxVolume = 1f;
        [SerializeField] private bool isSfxEnabled = true;
        [SerializeField] private bool isMusicEnabled = true;
        
        public float MasterVolume => masterVolume;
        public float MusicVolume => musicVolume;
        public float SfxVolume => sfxVolume;    
        public bool IsSfxEnabled => isSfxEnabled;
        public bool IsMusicEnabled => isMusicEnabled;
    }
}