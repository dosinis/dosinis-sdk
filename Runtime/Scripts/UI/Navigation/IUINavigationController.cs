using System;
using DosinisSDK.Core;

namespace DosinisSDK.UI.Navigation
{
    public interface IUINavigationController : IModule
    {
        public event Action<IUINavigationElement> OnCurrentElementChanged;
        /// <summary>
        /// <param name= "OnTabMovePerformed"> true - move next, false - move prev</param>
        /// </summary>
        public event Action<bool> OnTabMovePerformed; 
        public void RegisterElement(IUINavigationElement element);
        public void UnregisterElement(IUINavigationElement element);
        public void SetCurrentElement(IUINavigationElement element);
        public void RegisterCancellationElement(IUINavigationCancel element);
        public void UnregisterCancellationElement(IUINavigationCancel element);
        public void RebuildNavigation();
    }
}