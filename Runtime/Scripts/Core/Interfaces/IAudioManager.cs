using UnityEngine;

namespace DosinisSDK.Core
{
    public interface IAudioManager : IBehaviourModule
    {
        public void PlayOneShot(AudioClip clip, float volume = 1);
        public void PlayMusic(AudioClip clip);
        public void PlayLoop(AudioClip clip);
        public void StopLoop(AudioClip clip);
        public void SetSFXMuted(bool value);
        public void SetMusicMuted(bool value);
        public void SetVolume(float volume);

    }
}
