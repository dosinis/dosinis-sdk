using System;
using DosinisSDK.Assets;
using UnityEngine;

namespace DosinisSDK.Localization
{
    [Serializable]
    public class LocalizedAudioWrapper
    {
        [SerializeField] private SystemLanguage language;
        [SerializeField] private AssetLink audioClip;

        public SystemLanguage SystemLanguage => language;
        public AssetLink AudioClip => audioClip;
    }
    
    [Serializable] 
    public class LocalizedAudioClips
    {
        [SerializeField] private string key;
        [SerializeField] private LocalizedAudioWrapper[] localizedAudioClips;

        public string Key => key;
        
        public AssetLink GetLocalizedAudioClip(SystemLanguage language)
        {
            foreach (var localizedAudioClip in localizedAudioClips)
            {
                if (localizedAudioClip.SystemLanguage == language)
                {
                    return localizedAudioClip.AudioClip;
                }
            }

            return null;
        }
    }
}
