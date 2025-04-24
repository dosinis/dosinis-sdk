using DosinisSDK.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Audio
{
    public class AudioManager : BehaviourModule, IAudioManager
    {
        private readonly List<AudioSource> sources = new();
        private readonly List<AudioSource> worldSources = new();
        private AudioSource musicSource;
        private AudioData data;
        private bool silencingMusic = false;
        private float musicVolumeModifier = 1f;

        private const int POOL_SIZE = 10;

        public float SoundsVolume => IsSfxEnabled ? data.soundsVolume : 0;
        public float MusicVolume => IsMusicEnabled ? data.musicVolume * musicVolumeModifier : 0;
        public bool IsSfxEnabled => data.isSfxEnabled;
        public bool IsMusicEnabled => data.isMusicEnabled;
        
        public event Action<bool> OnMusicEnabled;
        public event Action<bool> OnSfxEnabled;
        public event Action<float> OnMusicVolumeChanged;
        public event Action<float> OnSfxVolumeChanged;

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
            
            AudioListener.volume = data.masterVolume;
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

        public AudioSource PlayMusic(AudioClip clip)
        {
            return PlayMusic(clip, 1f);
        }

        public AudioSource PlayMusic(AudioClip clip, float volume, float fadeDuration = 0f)
        {
            musicVolumeModifier = volume;
            
            SetMusicSourceVolume(MusicVolume);
            
            if (fadeDuration == 0)
            {
                musicSource.clip = clip;
                musicSource.Play();
            }
            else
            {
                StartCoroutine(FadeMusicCoroutine(clip, volume, fadeDuration));
            }

            return musicSource;
        }
        
        private IEnumerator FadeMusicCoroutine(AudioClip clip, float volume, float fadeDuration)
        {
            var halfDuration = fadeDuration / 2f;
            var t = 0f;
            var initVolume = musicSource.volume;
            
            while (t < halfDuration)
            {
                t += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(initVolume, 0f, t / halfDuration);
                yield return null;
            }

            musicSource.clip = clip;
            musicSource.Play();

            t = 0f;
            
            while (t < halfDuration)
            {
                t += Time.deltaTime;
                SetMusicSourceVolume(Mathf.Lerp(0f, volume, t / halfDuration));
                yield return null;
            }
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
                    src.volume = SoundsVolume * volume;
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
                    src.PlayOneShot(clip, SoundsVolume * volume);
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
                src.volume = SoundsVolume * volume;
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
                    src.volume = SoundsVolume * volume;
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
        
        public void SetMasterVolume(float volume)
        {
            AudioListener.volume = volume;
            data.masterVolume = volume;
        }

        public void SetMusicVolume(float volume)
        {
            data.musicVolume = volume;
            musicSource.volume = volume;
            
            OnMusicVolumeChanged?.Invoke(volume);
        }

        public void SetSoundsVolume(float volume)
        {
            data.soundsVolume = volume;
            
            foreach (var src in sources)
            {
                src.volume = volume;
            }
            
            foreach (var src in worldSources)
            {
                src.volume = volume;
            }
            
            OnSfxVolumeChanged?.Invoke(volume);
        }

        public void SetMusicEnabled(bool value)
        {
            data.isMusicEnabled = value;

            musicSource.volume = value ? MusicVolume : 0;
            
            OnMusicEnabled?.Invoke(value);
            OnMusicVolumeChanged?.Invoke(musicSource.volume);
        }
        
        public void SetSfxEnabled(bool value)
        {
            data.isSfxEnabled = value;
            float volume = value ? 1 : 0;

            foreach (var src in sources)
            {
                src.volume = volume;
            }

            foreach (var src in worldSources)
            {
                src.volume = volume;
            }
            
            OnSfxEnabled?.Invoke(value);
            OnSfxVolumeChanged?.Invoke(volume);
        }

        public void SetMusicPitch(float value)
        {
            musicSource.pitch = value;
        }
        
        private void SetMusicSourceVolume(float volume)
        {
            if (!data.isMusicEnabled)
                return;
            
            musicSource.volume = volume;
        }
    }
}
