using DosinisSDK.Audio;
using DosinisSDK.Core;

namespace DosinisSDK.UI.Utils
{
    public class SoundToggle : SwitcherButton
    {
        protected override void OnClick()
        {
            base.OnClick();
            var audioModule = App.Core.GetModule<IAudioManager>();

            audioModule.SetSfxEnabled(!audioModule.IsSfxEnabled);
        }
    }

}

