using DosinisSDK.Audio;
using DosinisSDK.Core;

namespace DosinisSDK.UI
{
    public class MusicToggle : SwitcherButton
    {
        protected override void OnClick()
        {
            base.OnClick();
            var audioModule = App.Core.GetCachedModule<IAudioManager>();

            audioModule.SetMusicMuted(!audioModule.IsMusicMuted);
        }
    }
}