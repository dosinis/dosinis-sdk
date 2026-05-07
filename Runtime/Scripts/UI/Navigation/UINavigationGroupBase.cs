using System;
using System.Collections.Generic;
using System.Linq;
using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public abstract class UINavigationGroupBase : UINavigationBase
    {
        [SerializeField] protected bool autoPopulateChildren = true;
        [SerializeField] protected List<GameObject> children = new();
        protected int currentIndex = 0;

        protected int ActiveChildrenCount => children.Count(CheckForChild);

        public override bool IsActiveNavigation
        {
            get => isActiveNavigation && ActiveChildrenCount > 0;
            protected set => isActiveNavigation = value;
        }

        public override GameObject Target
        {
            get
            {
                var activeChildren = children.Where(CheckForChild).ToList();
                if (activeChildren.Count == 0) return target;

                return activeChildren[currentIndex];
            }
        }

        private bool CheckForChild(GameObject gO)
        {
            if (gO.activeInHierarchy)
            {
                return !gO.TryGetComponent<IInteractableElement>(out var interactable) || interactable.Interactable;
            }

            return false;
        }

        public void SetChildren(IEnumerable<GameObject> childrenElements)
        {
            children.Clear();
            children.AddRange(childrenElements);
            navigationController?.SetCurrentElement(this);
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (!autoPopulateChildren) return;
            if (children.Count == 0)
            {
                foreach (Transform child in transform)
                {
                    children.Add(child.gameObject);
                }
            }
        }
#endif
    }
}