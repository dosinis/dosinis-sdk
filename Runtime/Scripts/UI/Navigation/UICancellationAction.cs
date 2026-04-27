using System;
using DosinisSDK.Core;
using DosinisSDK.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace DosinisSDK.UI.Navigation
{
    public class UICancellationAction : ManagedBehaviour, IUINavigationCancel
    {
        [SerializeField] private UnityEvent onCancel;
        private IUINavigationController navigationController;
        private bool IsEnabled => gameObject.activeInHierarchy;

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
            onCancel.Invoke();
        }
    }
}