using DosinisSDK.Core;

namespace DosinisSDK.UI
{
    public class OverlayWindow : FadingWindow
    {
        public static void ShowOverlay()
        {
            App.Core.GetCachedModule<UIManager>().GetWindow<OverlayWindow>().Show();
        }

        public static void HideOverlay()
        {
            App.Core.GetCachedModule<UIManager>().GetWindow<OverlayWindow>().Hide();
        }
    }
}
