using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    [RequireComponent(typeof(UIScrollElementNavigationBridge))]
    public class UIScrollFocusElement : MonoBehaviour, IUIScrollFocusElement
    {
        private IUIScrollFocusController controller;

        public void InitializeController(IUIScrollFocusController controller)
        {
            this.controller = controller;
        }

        public void SetSelected()
        {
            if (transform is RectTransform rectTransform)
            {
                controller.CheckAndScroll(rectTransform);
            }
        }
    }
}