using System;

namespace DosinisSDK.Core
{
    public class WindowWithArgs<T> : Window, IWindowWithArgs<T>
    {
        protected T args;
        
        public virtual void Show(T args, Action done = null, Action onHidden = null)
        {
            this.args = args;
            Show(done, onHidden);
        }
    }

    public interface IWindowWithArgs<in T> : IWindow
    {
        void Show(T args, Action done = null, Action onHidden = null);
    }
}


