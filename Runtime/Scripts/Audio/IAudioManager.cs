using System;
using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Audio
{
    public interface IAudioManager : IModule
    {
        event Action<bool> OnMusicEnabled;
        event Action<bool> OnSfxEnabled;
        void PlayOneShot(AudioClip clip, float volume = 1f, bool silentMusic = false);
        void PlayAtPoint(AudioClip clip, Vector3 position, float minDistance = 3f, float maxDistance = 100f, float volume = 1);
        void StopMusic();
        void PauseMusic();
        void ResumeMusic();
        void PlayMusic(AudioClip clip);
        void PlayMusic(AudioClip clip, float volume);
        void SetPlayingClipPitch(AudioClip clip, float pitch);
        bool IsPlaying(AudioClip clip);
        void PlayLoop(AudioClip clip, float volume = 1f);
        void StopLoop(AudioClip clip);
        void PlayClip(AudioClip clip, float volume = 1f);
        void StopClip(AudioClip clip);
        void SetMusicPitch(float value);
        void SetSfxEnabled(bool value);
        void SetMusicEnabled(bool value);
        void SetTimeScale(float value, float lerpDuration = 0.1f);
        void SetMusicSourceVolume(float volume);
        void SetVolume(float volume);
        bool IsSfxEnabled { get; }
        bool IsMusicEnabled { get; }
    }
}
