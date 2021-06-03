using DosinisSDK.Core;

namespace DosinisSDK.UI
{
    public class SoundToggle : SwitcherButton
    {
        protected override void OnClick()
        {
            base.OnClick();
            var audioModule = App.Core.GetCachedBehaviourModule<IAudioManager>();

            audioModule.SetSfxMuted(!audioModule.IsSfxMuted);
        }
    }

}

