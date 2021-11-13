using DosinisSDK.Core;

namespace DosinisSDK.UI
{
    public class OverlayWindow : AnimatedWindow
    {
        public static void ShowOverlay(int callerSiblingIndex)
        {
            var overlay = App.Core.GetCachedModule<UIManager>().GetWindow<OverlayWindow>();

            overlay.transform.SetSiblingIndex(callerSiblingIndex - 1);
            overlay.Show();
        }

        public static void HideOverlay()
        {
            var overlay = App.Core.GetCachedModule<UIManager>().GetWindow<OverlayWindow>();

            overlay.Hide(() =>
            {
                overlay.transform.SetAsFirstSibling();
            });
        }
    }
}
