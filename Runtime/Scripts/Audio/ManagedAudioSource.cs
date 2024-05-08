using DosinisSDK.Core;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class ManagedAudioSource : ManagedBehaviour
    {
        [SerializeField] private bool music = false;
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
            audioManager.OnSfxEnabled += OnSfxEnabled;
            audioManager.OnMusicEnabled += OnMusicEnabled;

            SetupVolume();
        }
        
        private void OnDestroy()
        {
            audioManager.OnSfxEnabled -= OnSfxEnabled;
            audioManager.OnMusicEnabled -= OnMusicEnabled;
        }

        private void Reset()
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        private void OnMusicEnabled(bool obj)
        {
            if (music)
            {
                SetupVolume();
            }
        }

        private void OnSfxEnabled(bool value)
        {
            if (!music)
            {
                SetupVolume();
            }
        }

        private void SetupVolume()
        {
            audioSource.volume = audioManager.IsSfxEnabled ? initVolume : 0;
        }
    }
}