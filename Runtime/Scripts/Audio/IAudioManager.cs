using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Audio
{
    public interface IAudioManager : IModule
    {
        void PlayOneShot(AudioClip clip, float volume = 1f, bool silentMusic = false);
        void PlayAtPoint(AudioClip clip, Vector3 position, float volume = 1f);
        void StopMusic();
        void PlayMusic(AudioClip clip, float volume = 1f);
        void PlayLoop(AudioClip clip);
        void StopLoop(AudioClip clip);
        void SetMusicPitch(float value);
        void SetSfxEnabled(bool value);
        void SetMusicEnabled(bool value);
        void SetTimeScale(float value, float lerpDuration = 0.1f);
        void SetVolume(float volume);
        bool IsSfxEnabled { get; }
        bool IsMusicEnabled { get; }
    }
}
