using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Audio
{
    public interface IAudioManager : IModule
    {
        void PlayOneShot(AudioClip clip, float volume = 1f, bool silentMusic = false);
        void PlayAtPoint(AudioClip clip, Vector3 position, float minDistance = 1f, float maxDistance = 500f, float volume = 1);
        void StopMusic();
        void PauseMusic();
        void ResumeMusic();
        void PlayMusic(AudioClip clip);
        void PlayMusic(AudioClip clip, float volume);
        void SetPlayingClipPitch(AudioClip clip, float pitch);
        void PlayLoop(AudioClip clip, float volume = 1f);
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
