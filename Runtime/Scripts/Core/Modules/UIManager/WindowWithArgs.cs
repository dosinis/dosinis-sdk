using System;

namespace DosinisSDK.Core
{
    public abstract class WindowWithArgs<T> : Window, IWindowWithArgs<T>
    {
        protected T args;
        
        public virtual void Show(T args, Action done = null, Action onHidden = null, Action onBeforeHide = null)
        {
            this.args = args;
            Show(done, onHidden, onBeforeHide);
        }

        public void ShowImmediately(T args, Action done=null, Action onHidden = null, Action onBeforeHide = null)
        {
            this.args = args;
            ShowImmediately(done,onHidden, onBeforeHide);
        }
    }

    public interface IWindowWithArgs<in T> : IWindow
    {
        void Show(T args, Action done = null, Action onHidden = null, Action onBeforeHide = null);
        void ShowImmediately(T args,Action done=null, Action onHidden = null, Action onBeforeHide = null);
    }
}


