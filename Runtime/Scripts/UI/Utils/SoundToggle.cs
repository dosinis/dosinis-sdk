using DosinisSDK.Audio;
using DosinisSDK.Core;

namespace DosinisSDK.UI
{
    public class SoundToggle : SwitcherButton
    {
        protected override void OnClick()
        {
            base.OnClick();
            var audioModule = App.Core.GetCachedModule<IAudioManager>();

            audioModule.SetSfxMuted(!audioModule.IsSfxMuted);
        }
    }

}

