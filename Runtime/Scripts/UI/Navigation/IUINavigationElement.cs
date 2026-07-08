using System;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public interface IUINavigationElement : IDisposable, IUINavigationCancel
    {
      
        public bool StartNavigationFromHere { get; }
        public GameObject Target { get; }
        public Observable<bool> IsSelected { get; } 
        public void Select();
        public void Deselect();
        public void Submit();
        public void Hold();
        public void SetActiveNavigation(bool value);
        public void Unhold();
        public void Move(Vector2 axis);
        public void SetNavigationElement(NavigationDirection direction, IUINavigationElement element);
        public bool TryGetNavigationElement(NavigationDirection direction, out IUINavigationElement element);
        public void SetStartNavigationFromHere(bool value);
    }
}