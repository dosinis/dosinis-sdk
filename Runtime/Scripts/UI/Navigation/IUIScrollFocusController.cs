using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public interface IUIScrollFocusController
    {
        public void CheckAndScroll(RectTransform target);

    }
}