using DosinisSDK.Core;

namespace DosinisSDK.UI.Navigation
{
    public interface IUINavigationController : IModule
    {
        public void RegisterElement(IUINavigationElement element);
        public void UnregisterElement(IUINavigationElement element);
        public void SetCurrentElement(IUINavigationElement element);
        public void RegisterCancellationElement(IUINavigationElement element);
        public void UnregisterCancellationElement();
        public void RebuildNavigation();
    }
}