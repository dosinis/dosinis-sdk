using System;
using UnityEngine;

namespace DosinisSDK.Core
{
    public abstract class BasePanel : MonoBehaviour, ISubWindowElement
    {
        protected bool Initialized;
        protected IWindow ParentWindow;
        private IWindowTransition windowTransition;


        public void Init(IApp app, IWindow parentWindow)
        {
            ParentWindow = parentWindow;
            ParentWindow.OnShown += OnWindowShown;
            ParentWindow.OnHidden += OnWindowHidden;
            ParentWindow.OnBeforeShow += OnWindowBeforeShow;
            ParentWindow.OnBeforeHide += OnWindowBeforeHide;
            OnInit(app);
        }

        public void Dispose()
        {
            ParentWindow.OnShown -= OnWindowShown;
            ParentWindow.OnHidden -= OnWindowHidden;
            ParentWindow.OnBeforeShow -= OnWindowBeforeShow;
            ParentWindow.OnBeforeHide -= OnWindowBeforeHide;
            OnDispose();
        }

        public void Show(Action done = null)
        {
            ShowInternal(done);
        }

        public void Hide()
        {
            HideInternal();
        }

        protected virtual void OnInit(IApp app)
        {
            Initialized = true;
            windowTransition = GetComponent<IWindowTransition>();
            windowTransition.Init();
        }

        protected virtual void ShowInternal(Action done)
        {
            if (!Initialized) return;
            OnBeforeShownInternal();
            if (windowTransition is not null)
            {
                windowTransition.ShowTransition((() =>
                {
                    done?.Invoke();
                    OnShownInternal();
                }));
            }
            else
            {
                done?.Invoke();
                OnShownInternal();
            }
        }

        protected virtual void HideInternal()
        {
            if (!Initialized) return;
            OnBeforeHiddenInternal();
            if (windowTransition is not null)
            {
                windowTransition.HideTransition(OnHiddenInternal);
            }
            else
            {
                OnHiddenInternal();
            }
        }

        protected virtual void OnBeforeShownInternal()
        {
        }

        protected virtual void OnBeforeHiddenInternal()
        {
        }

        protected virtual void OnShownInternal()
        {
            gameObject.SetActive(true);
        }

        protected virtual void OnHiddenInternal()
        {
            gameObject.SetActive(false);
        }

        protected virtual void OnWindowShown()
        {
        }

        protected virtual void OnWindowHidden()
        {
        }

        protected virtual void OnWindowBeforeShow()
        {
        }

        protected virtual void OnWindowBeforeHide()
        {
        }

        protected virtual void OnDispose()
        {
        }
    }
}