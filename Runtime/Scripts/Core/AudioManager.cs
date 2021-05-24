using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class AudioManager : BehaviourModule, IAudioManager
    {
        private int POOL_SIZE = 10;

        private List<AudioSource> sources = new List<AudioSource>();

        public override void Init(IApp app)
        {
            for (int i = 0; i < POOL_SIZE; i++)
            {
                var source = new GameObject();
                source.transform.SetParent(transform);
                source.name = "Source";
                sources.Add(source.AddComponent<AudioSource>());
            }

        }

        public void PlayLoop(AudioClip clip, float volume = 1)
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

        public void SetMuted(bool value)
        {
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
        }
    }
}
