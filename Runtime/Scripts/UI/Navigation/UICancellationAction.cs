using System;
using DosinisSDK.Core;
using UnityEngine;
using UnityEngine.Events;

namespace DosinisSDK.UI.Navigation
{
    public class UICancellationAction : UIAction, IUINavigationCancel
    {
        [SerializeField] private bool isActiveNavigation = true;
        [SerializeField] public UnityEvent onCancel;
        private IUINavigationController navigationController;
        public override bool IsEnabled => gameObject.activeInHierarchy;
        public override bool IsActiveNavigation => isActiveNavigation;

        public override void SetActiveNavigation(bool value)
        {
            isActiveNavigation = value;
        }

        protected override void OnInit(IApp app)
        {
            navigationController = app.GetModule<IUINavigationController>();
            if (IsEnabled)
            {
                navigationController.RegisterCancellationElement(this);
            }
        }

        private void OnEnable()
        {
            navigationController?.RegisterCancellationElement(this);
        }

        private void OnDisable()
        {
            navigationController?.UnregisterCancellationElement(this);
        }

        public void Cancel()
        {
            if (IsEnabled && IsActiveNavigation)
            {
                onCancel?.Invoke();
            }
        }
    }
}