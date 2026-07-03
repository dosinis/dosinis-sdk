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
        [SerializeField] protected bool onlyInteractableChildren = true;
        protected int currentIndex = 0;

        protected int ActiveChildrenCount => onlyInteractableChildren ? children.Count(CheckForChild) : children.Count;

        public override bool IsActiveNavigation => isActiveNavigation && ActiveChildrenCount > 0;

        public override GameObject Target
        {
            get
            {
                var activeChildren = onlyInteractableChildren ? children.Where(CheckForChild).ToList() : children;
                if (activeChildren.Count == 0) return target;
                currentIndex = Mathf.Clamp(currentIndex, 0, activeChildren.Count - 1);
                return activeChildren[currentIndex];
            }
        }

        protected bool CheckForChild(GameObject gO)
        {
            if (gO && gO.activeInHierarchy)
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

        public void AddChild(GameObject element)
        {
            children.Add(element);
            navigationController?.SetCurrentElement(this);
        }

        public void Cleanup()
        {
            children.Clear();
        }

        public void RemoveChild(GameObject element)
        {
            children.Remove(element);
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