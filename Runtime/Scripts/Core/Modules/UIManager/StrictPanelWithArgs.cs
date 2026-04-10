using System;

namespace DosinisSDK.Core
{
    public class StrictPanelWithArgs<TWindow, TArgs> : StrictPanel<TWindow>, IWindowElementWithArgs<TArgs> where TWindow : IWindow
    {
        protected TArgs Args;
        public void Show(TArgs args, Action done = null)
        {
            Args = args;
            Show(done);
        }
    }
}