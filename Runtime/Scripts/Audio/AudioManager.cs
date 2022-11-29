using DosinisSDK.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Audio
{
    public class AudioManager : BehaviourModule, IAudioManager
    {
        [SerializeField] private float musicVolume = 1f;

        private const int POOL_SIZE = 10;

        private List<AudioSource> sources = new List<AudioSource>();

        private AudioSource musicSource;

        private AudioData data;

        private bool silencingMusic = false;

        public bool IsSfxEnabled => data.isSfxEnabled;
        public bool IsMusicEnabled => data.isMusicEnabled;

        protected override void OnInit(IApp app)
        {
            data = app.Data.RetrieveOrCreateData<AudioData>();

            for (int i = 0; i < POOL_SIZE; i++)
            {
                var source = new GameObject();
                source.transform.SetParent(transform);
                source.name = "Source";
                sources.Add(source.AddComponent<AudioSource>());
            }

            var mSource = new GameObject();
            mSource.transform.SetParent(transform);
            mSource.name = "MusicSource";

            musicSource = mSource.AddComponent<AudioSource>();
            musicSource.loop = true;

            SetMusicEnabled(data.isMusicEnabled);
            SetSfxEnabled(data.isSfxEnabled);
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void PlayMusic(AudioClip clip, float volume = 1f)
        {
            musicVolume = volume;
            musicSource.clip = clip;
            
            if (data.isMusicEnabled)
                musicSource.volume = volume;

            musicSource.Play();
        }

        public void PlayLoop(AudioClip clip)
        {
            foreach (AudioSource src in sources)
            {
                if (src.isPlaying == false)
                {
                    src.clip = clip;
                    src.loop = true;
                    src.Play();
                    break;
                }
            }
        }

        public void StopLoop(AudioClip clip)
        {
            foreach (AudioSource src in sources)
            {
                if (src.isPlaying && src.clip == clip)
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

            while (t < 1)
            {
                musicSource.volume = Mathf.Lerp(musicSource.volume, currentVolume / 2f, t);
                t += Time.deltaTime / duration;
                yield return null;
            }

            onSilent?.Invoke();

            yield return new WaitForSeconds(effectDuration);

            t = 0;

            while (t < 1)
            {
                musicSource.volume = Mathf.Lerp(musicSource.volume, currentVolume, t);
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
                foreach (AudioSource src in sources)
                {
                    if (src.isPlaying == false)
                    {
                        src.PlayOneShot(clip, volume);
                        break;
                    }
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
        }

        public void SetTimeScale(float value, float lerpDuration = 0.1f)
        {
            StartCoroutine(LerpTimeScale(value, lerpDuration));
        }

        private IEnumerator LerpTimeScale(float value, float lerpDuration)
        {
            float duration = lerpDuration;

            while (duration > 0)
            {
                duration -= Time.deltaTime;
                
                SetMusicPitch(Mathf.Lerp(musicSource.pitch, value, lerpDuration / duration));
                foreach (var src in sources)
                {
                    src.pitch = Mathf.Lerp(src.pitch, value, lerpDuration / duration);
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
        }

        public void SetMusicPitch(float value)
        {
            musicSource.pitch = value;
        }
    }
}
