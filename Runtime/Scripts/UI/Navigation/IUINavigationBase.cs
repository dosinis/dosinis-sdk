namespace DosinisSDK.UI.Navigation
{
    public interface IUINavigationBase
    {
        public bool IsEnabled { get; }
        public bool IsActiveNavigation { get; }
        public void SetActiveNavigation(bool value);
    }
}
