using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Audio
{
    public interface IAudioManager : IBehaviourModule
    {
        public void PlayOneShot(AudioClip clip, float volume = 1);
        public void StopMusic();
        public void PlayMusic(AudioClip clip);
        public void PlayLoop(AudioClip clip);
        public void StopLoop(AudioClip clip);
        public void SetSfxEnabled(bool value);
        public void SetMusicEnabled(bool value);
        public void SetVolume(float volume);
        bool IsSfxEnabled { get; }
        bool IsMusicEnabled { get; }
    }
}
