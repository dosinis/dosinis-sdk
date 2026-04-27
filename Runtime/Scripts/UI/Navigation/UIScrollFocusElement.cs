using DosinisSDK.Core;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public class UIScrollFocusElement : ManagedBehaviour, IUIScrollFocusElement
    {
        private IUINavigationElement navigationElement;
        private IUINavigationController navigationController;
        private IUIScrollFocusController scrollFocusController;

        protected override void OnInit(IApp app)
        {
            navigationElement = GetComponent<IUINavigationElement>();
            if (navigationElement == null) return;
            navigationController = app.GetModule<IUINavigationController>();
            if (navigationController != null && navigationElement.IsEnabled)
            {
                navigationController.OnCurrentElementChanged += OnCurrentElementChanged;
            }
        }

        public void InitializeController(IUIScrollFocusController controller)
        {
            scrollFocusController = controller;
        }

        public void SetSelected()
        {
            if (navigationElement.Target.transform is RectTransform rectTransform)
            {
                scrollFocusController.CheckAndScroll(rectTransform);
            }
        }

        private void OnCurrentElementChanged(IUINavigationElement element)
        {
            if (element.Equals(navigationElement))
            {
                SetSelected();
            }
        }

        private void OnEnable()
        {
            if (navigationController == null) return;
            navigationController.OnCurrentElementChanged += OnCurrentElementChanged;
        }

        private void OnDisable()
        {
            if (navigationController == null) return;
            navigationController.OnCurrentElementChanged -= OnCurrentElementChanged;
        }
    }
}