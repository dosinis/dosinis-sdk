using System;
using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Audio
{
    public interface IAudioManager : IModule
    {
        event Action<bool> OnMusicEnabled;
        event Action<bool> OnSfxEnabled;
        event Action<float> OnMusicVolumeChanged;
        event Action<float> OnSfxVolumeChanged;
        void PlayOneShot(AudioClip clip, float volume = 1f, bool silentMusic = false, float pitch = 1f);
        void PlayAtPoint(AudioClip clip, Vector3 position, float minDistance = 3f, float maxDistance = 100f, float volume = 1, float pitch = 1f);
        void StopMusic();
        void PauseMusic();
        void ResumeMusic();
        AudioSource PlayMusic(AudioClip clip);
        AudioSource PlayMusic(AudioClip clip, float volume, float fadeDuration = 0, float pitch = 1f);
        void SetPlayingClipPitch(AudioClip clip, float pitch);
        bool IsPlaying(AudioClip clip);
        void PlayLoop(AudioClip clip, float volume = 1f, float pitch = 1f);
        void StopLoop(AudioClip clip);
        void PlayClip(AudioClip clip, float volume = 1f, float pitch = 1f);
        void StopClip(AudioClip clip);
        void SetMusicPitch(float value);
        void SetSfxEnabled(bool value);
        void SetMusicEnabled(bool value);
        void SetTimeScale(float value, float lerpDuration = 0.1f);
        void SetMasterVolume(float volume);
        void SetMusicVolume(float volume);
        void SetSoundsVolume(float volume);
        float MusicVolume { get; }
        float SoundsVolume { get; }
        bool IsSfxEnabled { get; }
        bool IsMusicEnabled { get; }
    }
}
