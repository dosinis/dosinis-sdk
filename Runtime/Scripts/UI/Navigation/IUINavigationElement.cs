using System;
using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public interface IUINavigationElement : IDisposable
    {
        public bool IsEnabled { get; }
        public bool StartNavigationFromHere { get; }
        public GameObject Target { get; }
        public void Select();
        public void Deselect();
        public void Submit();
        public void Cancel();
        public void Hold();
        public void Unhold();
        public void Move(Vector2 axis);
        
        public void SetStartNavigationFromHere(bool value);
    }
}