using DosinisSDK.Config;
using DosinisSDK.Model;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class AudioManager : BehaviourModule, IAudioManager
    {

        private const int POOL_SIZE = 10;

        private List<AudioSource> sources = new List<AudioSource>();

        private AudioSource musicSource;

        private IDataManager dataManager;

        private AudioData data;

        public override void Init(IApp app)
        {
            dataManager = app.GetCachedBehaviourModule<IDataManager>();

            data = dataManager.LoadData<AudioData>();

            dataManager.RegisterData(data);

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
        }

        public void StopMusic(AudioClip clip)
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

        public void SetSFXMuted(bool value)
        {
            data.isSfxEnabled = !value;
            if (value)
            {
                AudioListener.volume = 0;
            }
            else
            {
                AudioListener.volume = 1;
            }
        }

        public void SetVolume(float volume)
        {
            AudioListener.volume = volume;
            data.volume = volume;
        }

        public void SetMusicMuted(bool value)
        {
            data.isMusicEnabled = !value;
            if (value)
            {
                AudioListener.volume = 0;
            }
            else
            {
                AudioListener.volume = 1;
            }
        }
    }
}
