using DosinisSDK.Audio;
using DosinisSDK.Core;

namespace DosinisSDK.UI.Utils
{
    public class MusicToggle : SwitcherButton
    {
        protected override void OnClick()
        {
            base.OnClick();
            var audioModule = App.Core.GetCachedModule<IAudioManager>();

            audioModule.SetMusicEnabled(!audioModule.IsMusicEnabled);
        }
    }
}