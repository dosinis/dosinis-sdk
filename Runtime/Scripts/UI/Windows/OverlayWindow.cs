using DosinisSDK.Core;
using System.Collections.Generic;

namespace DosinisSDK.UI
{
    public class OverlayWindow : AnimatedWindow
    {
        private static Queue<Window> overlayQueue = new Queue<Window>();

        public static void ShowOverlay(Window caller)
        {
            //if (overlayQueue.Contains(caller))
            //    return;

            //overlayQueue.Enqueue(caller);

            //// this is wrong!!!
            //var overlay = App.Core.SceneManager.GetSingletonOfType<UIManager>().GetWindow<OverlayWindow>(); // TODO: IUIManager?

            //var callerSiblingId = caller.transform.GetSiblingIndex();

            //if (overlay.isActiveAndEnabled == false)
            //{
            //    overlay.transform.SetSiblingIndex(callerSiblingId - 1);
            //    overlay.Show();
            //}
        }

        public static void HideOverlay()
        {
            // this is wrong!!!
            //var overlay = App.Core.SceneManager.GetSingletonOfType<UIManager>().GetWindow<OverlayWindow>(); // TODO: IUIManager?

            //overlayQueue.Dequeue();

            //App.Core.Timer.SkipFrame(() => 
            //{
            //    if (overlayQueue.Count > 0)
            //    {
            //        overlay.transform.SetSiblingIndex(overlayQueue.Peek().transform.GetSiblingIndex() - 1);
            //    }
            //    else
            //    {
            //        if (overlay.isActiveAndEnabled)
            //        {
            //            overlay.Hide(() =>
            //            {
            //                overlay.transform.SetAsFirstSibling();
            //            });
            //        }
            //    }
            //});
                      
        }
    }
}
