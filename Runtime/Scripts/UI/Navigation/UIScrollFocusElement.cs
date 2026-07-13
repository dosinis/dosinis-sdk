using System;
using DosinisSDK.Core;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public class UIScrollFocusElement : ManagedBehaviour, IUIScrollFocusElement, IDisposable
    {
        private IUINavigationElement navigationElement;
        private IUINavigationController navigationController;
        private IUIScrollFocusController scrollFocusController;

        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform != null) return rectTransform;

                rectTransform = GetComponent<RectTransform>();

                return rectTransform;
            }
        }

        private RectTransform rectTransform;

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
            scrollFocusController.CheckAndScroll(this);
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

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (navigationController == null) return;
            navigationController.OnCurrentElementChanged -= OnCurrentElementChanged;
        }
    }
}