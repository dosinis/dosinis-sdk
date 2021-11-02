using DosinisSDK.Core;
using System.Collections.Generic;
using UnityEngine;
#if ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif

namespace DosinisSDK.Audio
{
    public class AudioManager : BehaviourModule, IAudioManager
    {
        private const int POOL_SIZE = 10;

        private List<AudioSource> sources = new List<AudioSource>();

        private AudioSource musicSource;

        private AudioData data;
        public bool IsSfxEnabled => data.isSfxEnabled;
        public bool IsMusicEnabled => data.isMusicEnabled;

        public override void Init(IApp app)
        {
            data = app.GetCachedModule<IDataManager>().LoadAndRegisterData<AudioData>();

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

        public void PlayMusic(AudioClip clip)
        {
            musicSource.clip = clip;
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

        public void PlayOneShot(AudioClip clip, float volume = 1)
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

        public void SetSfxEnabled(bool value)
        {
            data.isSfxEnabled = value;

            foreach (var src in sources)
            {
                src.volume = value ? 0 : 1;
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

            if (value)
            {
                musicSource.volume = 0;
            }
            else
            {
                musicSource.volume = 1;
            }
        }

    #if ADDRESSABLES
        public async void PlayOneShotAsync(AssetReferenceT<AudioClip> clipRef, float volume = 1)
        {
            var clip = await Addressables.LoadAssetAsync<AudioClip>(clipRef).Task;
            PlayOneShot(clip, volume);
        }

        public async void PlayMusicAsync(AssetReferenceT<AudioClip> clipRef)
        {
            var clip = await Addressables.LoadAssetAsync<AudioClip>(clipRef).Task;
            PlayMusic(clip);
        }
    #endif
    }
}
