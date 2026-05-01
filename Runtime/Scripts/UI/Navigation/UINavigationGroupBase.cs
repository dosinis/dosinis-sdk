using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public abstract class UINavigationGroupBase : UINavigationBase
    {
        [SerializeField] protected bool autoPopulateChildren = true;
        [SerializeField] protected List<GameObject> children = new();
        protected int ActiveChildrenCount => children.Count(o => o.activeInHierarchy);
        protected int currentIndex = 0;

        public override bool IsActiveNavigation
        {
            get => isActiveNavigation && ActiveChildrenCount > 0;
            protected set => isActiveNavigation = value;
        }

        public override GameObject Target
        {
            get
            {
                var activeChildren = children.Where(o => o.activeInHierarchy).ToList();
                if (activeChildren.Count == 0) return target;

                return activeChildren[currentIndex];
            }
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