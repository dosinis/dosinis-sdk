using DosinisSDK.Core;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class ManagedAudioSource : ManagedBehaviour
    {
        [SerializeField] private bool music;
        [SerializeField] private AudioSource audioSource;
        
        private float initVolume;
        private IAudioManager audioManager;

        protected override void OnInit(IApp app)
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }

            initVolume = audioSource.volume;

            audioManager = app.GetModule<IAudioManager>();
            
            audioManager.OnMusicVolumeChanged += OnMusicVolumeChanged;
            audioManager.OnSfxVolumeChanged += OnSfxVolumeChanged;

            OnMusicVolumeChanged(audioManager.MusicVolume);
            OnSfxVolumeChanged(audioManager.SoundsVolume);
        }
        
        private void OnDestroy()
        {
            if (initialized == false)
                return;
            
            audioManager.OnMusicVolumeChanged -= OnMusicVolumeChanged;
            audioManager.OnSfxVolumeChanged -= OnSfxVolumeChanged;
        }

        private void Reset()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnMusicVolumeChanged(float volume)
        {
            if (music)
            {
                SetVolume(volume);
            }
        }
        
        private void OnSfxVolumeChanged(float volume)
        {
            if (!music)
            {
                SetVolume(volume);
            }
        }

        private void SetVolume(float volume)
        {
            audioSource.volume = Mathf.Lerp(0f, initVolume, volume);
        }
    }
}