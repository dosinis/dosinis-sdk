using System;
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
        
        private void OnTabMovePerformed(bool obj)
        {
            if (!IsEnabled || !IsActiveNavigation) return;
            if (requireTabSelection && !isSelected) return;
            Deselect();
            int toAdd = obj ? 1 : -1;
            currentIndex = Math.Clamp(currentIndex + toAdd, 0, ActiveChildrenCount - 1);
            navigationController.SetCurrentElement(this);
            if (submitOnSelection)
            {
                Submit();
            }
        }
    }
}