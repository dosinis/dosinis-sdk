namespace DosinisSDK.UI.Navigation
{
    public interface IUIScrollFocusElement
    {
        public void InitializeController(IUIScrollFocusController controller);
        public void SetSelected();
    }
}