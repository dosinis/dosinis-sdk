using DosinisSDK.Core;
using DosinisSDK.Utils;

namespace DosinisSDK.UI.Navigation
{
    public class UIScrollElementNavigationBridge : ManagedBehaviour
    {
        private IUINavigationElement navigationElement;
        private IUIScrollFocusElement scrollFocusElement;
        private IUINavigationController navigationController;

        protected override void OnInit(IApp app)
        {
            navigationElement = GetComponent<IUINavigationElement>();
            scrollFocusElement = GetComponent<IUIScrollFocusElement>();
            if (navigationElement != null && scrollFocusElement != null)
            {
                navigationController = app.GetModule<IUINavigationController>();
                navigationController.OnCurrentElementChanged += OnCurrentElementChanged;
            }
        }

        private void OnCurrentElementChanged(IUINavigationElement element)
        {
            if (element.Equals(navigationElement))
            {
                scrollFocusElement.SetSelected();
            }
        }
    }
}