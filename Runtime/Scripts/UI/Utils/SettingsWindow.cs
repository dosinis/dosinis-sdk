using DosinisSDK.Audio;
using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.UI
{
    public class SettingsWindow : FadingWindow
    {
        [SerializeField] private SoundToggle soundToggle;
        [SerializeField] private MusicToggle musicToggle;

        public override void Init(IUIManager uIManager)
        {
            base.Init(uIManager);

            var audioManager = App.Core.GetCachedModule<IAudioManager>();

            soundToggle.Init(audioManager.IsSfxEnabled);
            musicToggle.Init(audioManager.IsMusicEnabled);
        }
    }
}
