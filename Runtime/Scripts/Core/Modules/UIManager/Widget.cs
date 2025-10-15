using System;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class Widget : MonoBehaviour
    {
        protected Window parentWindow;
        private bool isInitialized;

        internal void Init(IApp app, Window parentWindow)
        {
            this.parentWindow = parentWindow;
            parentWindow.OnShown += OnWindowShown;
            parentWindow.OnHidden += OnWindowHidden;
            parentWindow.OnBeforeShow += OnWindowBeforeShow;
            parentWindow.OnBeforeHide += OnWindowBeforeHide;

            OnInit(app);

            isInitialized = true;
        }

        internal void Dispose()
        {
            OnDispose();
        }

        private void Update()
        {
            if (!isInitialized)
                return;
            
            OnProcess(Time.deltaTime);
        }

        protected virtual void OnInit(IApp app)
        {
        }

        protected virtual void OnDispose()
        {
        }
        
        protected virtual void OnProcess(float delta)
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