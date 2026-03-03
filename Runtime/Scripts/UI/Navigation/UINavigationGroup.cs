using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public abstract class UINavigationGroupBase : UINavigationBase
    {
        [SerializeField] protected List<GameObject> children = new();
        protected int currentIndex = 0;
        public override GameObject Target => children[currentIndex];
        
    }
}