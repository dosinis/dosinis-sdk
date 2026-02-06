using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public class UINavigationGroup : UINavigationBase
    {
        [SerializeField] private List<GameObject> children = new();
        private int currentIndex = 0;
        
        
    }
}