using DosinisSDK.Audio;
using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.UI.Utils
{
    public class SettingsWindow : Window
    {
        [SerializeField] private SoundToggle soundToggle;
        [SerializeField] private MusicToggle musicToggle;

        public override void Init(IUIManager uiManager)
        {
            base.Init(uiManager);

            var audioManager = App.Core.GetModule<IAudioManager>();

            soundToggle.Init(audioManager.IsSfxEnabled);
            musicToggle.Init(audioManager.IsMusicEnabled);
        }
    }
}
