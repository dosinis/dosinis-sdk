using System;

namespace DosinisSDK.Core
{
    public abstract class GenericPanelWithArgs<TArgs> : GenericPanel, ISubWindowElementWithArgs<TArgs>
    {
        protected TArgs Args;
        public void Show(TArgs args, Action done = null)
        {
            Args = args;
            Show(done);
        }
    }
}