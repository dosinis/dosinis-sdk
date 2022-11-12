using UnityEngine;

namespace DosinisSDK.Core
{
    public class Widget : MonoBehaviour
    {
        protected Window parentWindow;

        internal void Init(Window parentWindow)
        {
            this.parentWindow = parentWindow;
            parentWindow.OnShown += OnWindowShown;
            parentWindow.OnHidden += OnWindowHidden;
            parentWindow.OnBeforeShow += OnWindowBeforeShow;
            parentWindow.OnBeforeHide += OnWindowBeforeHide;

            OnInit();
        }

        internal void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnDispose()
        {
        }

        protected virtual void OnWindowBeforeShow()
        {
        }

        protected virtual void OnWindowBeforeHide()
        {
        }

        protected virtual void OnWindowShown()
        {
        }

        protected virtual void OnWindowHidden()
        {
        }
    }

    public class WidgetFor<T> : Widget
    {
        protected T target;

        public virtual void Setup(T target)
        {
            this.target = target;
        }
    }
}