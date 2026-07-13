using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public class UINavigationParent : UINavigationBase
    {
        [SerializeField] private List<GameObject> children = new();

        public override GameObject Target => children.FirstOrDefault(go => go.activeInHierarchy) ?? target;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
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