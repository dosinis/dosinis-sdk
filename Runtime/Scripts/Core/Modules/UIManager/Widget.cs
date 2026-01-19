using System;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class Widget : MonoBehaviour, ISubWindowElement
    {
        protected IWindow parentWindow;
        private bool isInitialized;

        public void Init(IApp app, IWindow parentWindow)
        {
            this.parentWindow = parentWindow;
            parentWindow.OnShown += OnWindowShown;
            parentWindow.OnHidden += OnWindowHidden;
            parentWindow.OnBeforeShow += OnWindowBeforeShow;
            parentWindow.OnBeforeHide += OnWindowBeforeHide;

            OnInit(app);

            isInitialized = true;
        }

        public void Dispose()
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
        public virtual void Show(Action done = null)
        {
            gameObject.SetActive(true);
            done?.Invoke();
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
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