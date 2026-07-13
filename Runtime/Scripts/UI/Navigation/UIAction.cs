using DosinisSDK.Utils;

namespace DosinisSDK.UI.Navigation
{
    public abstract class UIAction : ManagedBehaviour, IUINavigationBase
    {
        public abstract bool IsEnabled { get; }
        public abstract bool IsActiveNavigation { get; }
        public abstract void SetActiveNavigation(bool value);
    }
}
