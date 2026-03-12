using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public class UIScrollFocusElement : MonoBehaviour, IUIScrollFocusElement
    {
        private UIScrollFocusController controller;

        public void InitializeController(UIScrollFocusController controller)
        {
            this.controller = controller;
        }

        public void SetSelected()
        {
            controller.CheckAndScroll(transform);
        }
    }
}