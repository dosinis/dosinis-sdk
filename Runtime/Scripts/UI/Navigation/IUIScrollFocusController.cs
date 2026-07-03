using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public interface IUIScrollFocusController
    {
        public void CheckAndScroll(IUIScrollFocusElement element);
        public void AddUIFocusElement(IUIScrollFocusElement element);
        public void RemoveUIFocusElement(IUIScrollFocusElement element);

    }
}