using UnityEngine;

namespace DosinisSDK.Core
{
    public interface IAudioManager : IBehaviourModule
    {
        public void PlayOneShot(AudioClip clip, float volume = 1);
        public void PlayLoop(AudioClip clip, float volume = 1);
        public void StopLoop(AudioClip clip);
        public void SetMuted(bool value);
        public void SetVolume(float volume);

    }
}
