using System;
using Cysharp.Threading.Tasks;
using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public class UITabNavigation : UINavigationGroupBase
    {
        [SerializeField] protected bool submitOnSelection = true;
        [SerializeField] protected bool requireTabSelection = true;


        protected override void OnInit(IApp app)
        {
            base.OnInit(app);
            navigationController.OnTabMovePerformed += OnTabMovePerformed;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            DeselectAll();
            currentIndex = 0;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            navigationController.OnTabMovePerformed -= OnTabMovePerformed;
            cts?.Cancel();
            cts?.Dispose();
        }

        private void OnTabMovePerformed(bool obj)
        {
            if (!IsEnabled || !IsActiveNavigation) return;
            if (requireTabSelection && !IsSelected.Value) return;
            Unhold();
            Deselect();
            int toAdd = obj ? 1 : -1;
            currentIndex = Math.Clamp(currentIndex + toAdd, 0, ActiveChildrenCount - 1);
            Hold();
            navigationController.SetCurrentElement(this);
            if (submitOnSelection)
            {
                SimulateSubmit().Forget();
            }
        }
    }
}