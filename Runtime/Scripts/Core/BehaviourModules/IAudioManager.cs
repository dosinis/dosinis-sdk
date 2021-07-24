using UnityEngine;
#if ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif

namespace DosinisSDK.Core
{
    public interface IAudioManager : IBehaviourModule
    {
        public void PlayOneShot(AudioClip clip, float volume = 1);

#if ADDRESSABLES
        public void PlayOneShotAsync(AssetReferenceT<AudioClip> clipRef, float volume = 1);
        public void PlayMusicAsync(AssetReferenceT<AudioClip> clipRef);
#endif
        public void PlayMusic(AudioClip clip);
        public void PlayLoop(AudioClip clip);
        public void StopLoop(AudioClip clip);
        public void SetSfxMuted(bool value);
        public void SetMusicMuted(bool value);
        public void SetVolume(float volume);
        bool IsSfxMuted { get; }
        bool IsMusicMuted { get; }
    }
}
