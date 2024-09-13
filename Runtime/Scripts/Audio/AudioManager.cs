using DosinisSDK.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Audio
{
    public class AudioManager : BehaviourModule, IAudioManager
    {
        private float musicVolume = 1f;
        private readonly List<AudioSource> sources = new();
        private readonly List<AudioSource> worldSources = new();
        private AudioSource musicSource;
        private AudioData data;
        private bool silencingMusic = false;

        private const int POOL_SIZE = 10;
        
        public bool IsSfxEnabled => data.isSfxEnabled;
        public bool IsMusicEnabled => data.isMusicEnabled;
        
        public event Action<bool> OnMusicEnabled;
        public event Action<bool> OnSfxEnabled;

        protected override void OnInit(IApp app)
        {
            data = app.DataManager.GetOrCreateData<AudioData>();

            for (int i = 0; i < POOL_SIZE; i++)
            {
                var sourceGo = new GameObject("Source");
                sourceGo.transform.SetParent(transform);
                var src = sourceGo.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.Stop(); // NOTE: hack to overcome Unity issue, where AudioSource isPlaying up until Start() is called.
                sources.Add(src);
                
                var worldSourceGo = new GameObject("WorldSource");
                worldSourceGo.transform.SetParent(transform);
                var worldSrc = worldSourceGo.AddComponent<AudioSource>();
                worldSrc.playOnAwake = false;
                worldSrc.spatialBlend = 1;
                worldSrc.Stop(); // NOTE: hack to overcome Unity issue, where AudioSource isPlaying up until Start() is called.
                worldSources.Add(worldSrc);
            }

            var mSource = new GameObject("MusicSource");
            mSource.transform.SetParent(transform);

            musicSource = mSource.AddComponent<AudioSource>();
            musicSource.loop = true;

            SetMusicEnabled(data.isMusicEnabled);
            SetSfxEnabled(data.isSfxEnabled);
        }
        
        public void StopMusic()
        {
            if (musicSource)
            {
                musicSource.Stop();
            }
        }
        
        public void PauseMusic()
        {
            musicSource.Pause();
        }

        public void ResumeMusic()
        {
            musicSource.Play();
        }

        public void PlayMusic(AudioClip clip)
        {
            musicSource.clip = clip;
            
            if (data.isMusicEnabled)
                musicSource.volume = musicVolume;

            musicSource.Play();
        }

        public void PlayMusic(AudioClip clip, float volume)
        {
            musicVolume = volume;
            musicSource.clip = clip;
            
            if (data.isMusicEnabled)
                musicSource.volume = volume;

            musicSource.Play();
        }

        public void SetMusicSourceVolume(float volume)
        {
            if (data.isMusicEnabled)
                musicSource.volume = volume;
        }

        public void SetPlayingClipPitch(AudioClip clip, float pitch)
        {
            foreach (var src in sources)
            {
                if (src.isPlaying && src.clip == clip)
                {
                    src.pitch = pitch;
                    break;
                }
            }
        }

        public bool IsPlaying(AudioClip clip)
        {
            foreach (var src in sources)
            {
                if (src.isPlaying && src.clip == clip)
                {
                    return true;
                }
            }

            foreach (var src in worldSources)
            {
                if (src.isPlaying && src.clip == clip)
                {
                    return true;
                }
            }
            
            return false;
        }

        public void PlayLoop(AudioClip clip, float volume = 1f)
        {
            foreach (var src in sources)
            {
                if (src.isPlaying == false)
                {
                    src.pitch = 1f;
                    src.clip = clip;
                    src.loop = true;
                    src.volume = volume;
                    src.Play();
                    break;
                }
            }
        }

        public void StopLoop(AudioClip clip)
        {
            foreach (var src in sources)
            {
                if (src && src.gameObject && src.isPlaying && src.clip == clip)
                {
                    src.Stop();
                    break;
                }
            }
        }

        private IEnumerator MusicSilentCoroutine(float effectDuration, Action onSilent)
        {
            silencingMusic = true;

            var duration = 0.2f;

            float t = 0f;

            var currentVolume = musicSource.volume;
            var targetVolume = currentVolume / 2f;

            while (t < 1)
            {
                musicSource.volume = Mathf.Lerp(currentVolume, targetVolume, t);
                t += Time.deltaTime / duration;
                yield return null;
            }

            onSilent?.Invoke();

            yield return new WaitForSeconds(effectDuration);

            t = 0;

            while (t < 1)
            {
                musicSource.volume = Mathf.Lerp(targetVolume, currentVolume, t);
                t += Time.deltaTime / duration;
                yield return null;
            }

            silencingMusic = false;
        }

        public void PlayOneShot(AudioClip clip, float volume = 1, bool silentMusic = false)
        {
            if (silentMusic && silencingMusic == false)
            {
                StartCoroutine(MusicSilentCoroutine(clip.length, play));
            }
            else
            {
                play();
            }

            void play()
            {
                foreach (var src in sources)
                {
                    if (src.isPlaying) 
                        continue;
                    
                    src.pitch = 1f;
                    src.PlayOneShot(clip, volume);
                    break;
                }
            }
        }
        
        public void PlayAtPoint(AudioClip clip, Vector3 position, float minDistance = 1f, float maxDistance = 500f, float volume = 1)
        {
            foreach (var src in worldSources)
            {
                if (src.isPlaying)
                    continue;
                
                src.transform.position = position;
                
                src.clip = clip;
                src.volume = volume;
                src.minDistance = minDistance;
                src.maxDistance = maxDistance;
                src.pitch = 1f;
            
                src.Play();
                break;
            }
        }
        
        public void PlayClip(AudioClip clip, float volume = 1)
        {
            foreach (var src in sources)
            {
                if (src.isPlaying == false)
                {
                    src.pitch = 1f;
                    src.clip = clip;
                    src.volume = volume;
                    src.Play();
                    break;
                }
            }
        }

        public void StopClip(AudioClip clip)
        {
            foreach (var src in sources)
            {
                if (src.isPlaying && src.clip == clip)
                {
                    src.Stop();
                }
            }
        }

        public void SetSfxEnabled(bool value)
        {
            data.isSfxEnabled = value;

            foreach (var src in sources)
            {
                src.volume = value ? 1 : 0;
            }

            foreach (var src in worldSources)
            {
                src.volume = value ? 1 : 0;
            }
            
            OnSfxEnabled?.Invoke(value);
        }

        public void SetTimeScale(float value, float lerpDuration = 0.1f)
        {
            StartCoroutine(LerpTimeScale(value, lerpDuration));
        }

        private IEnumerator LerpTimeScale(float value, float lerpDuration)
        {
            float duration = lerpDuration;
            var initPitch = musicSource.pitch;

            while (duration > 0)
            {
                duration -= Time.deltaTime;
                
                SetMusicPitch(Mathf.Lerp(initPitch, value, lerpDuration / duration));
                
                var initPitches = new List<float>();
                
                foreach (var src in sources)
                {
                    initPitches.Add(src.pitch);
                }

                int i = 0;
                foreach (var src in sources)
                {
                    src.pitch = Mathf.Lerp(initPitches[i], value, lerpDuration / duration);
                    i++;
                }
                
                yield return null;
            }
        }
        
        public void SetVolume(float volume)
        {
            AudioListener.volume = volume;
            data.volume = volume;
        }

        public void SetMusicEnabled(bool value)
        {
            data.isMusicEnabled = value;

            musicSource.volume = value ? musicVolume : 0;
            
            OnMusicEnabled?.Invoke(value);
        }

        public void SetMusicPitch(float value)
        {
            musicSource.pitch = value;
        }
    }
}
