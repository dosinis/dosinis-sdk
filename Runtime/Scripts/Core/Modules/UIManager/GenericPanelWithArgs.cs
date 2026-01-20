using System;

namespace DosinisSDK.Core
{
    public abstract class GenericPanelWithArgs<TArgs> : GenericPanel, ISubWindowElementWithArgs<TArgs>
    {
        protected TArgs args;
        
        public void Show(TArgs args, Action done = null)
        {
            this.args = args;
            Show(done);
        }
    }
}