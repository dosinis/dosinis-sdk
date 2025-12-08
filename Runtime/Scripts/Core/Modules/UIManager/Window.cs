using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Core
{
    public abstract class Window : MonoBehaviour, IWindow
    {
#if UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
        [SerializeField] protected bool ignoreSafeArea = false;
#endif
        [SerializeField] protected Button closeButton;
        [SerializeField] private bool isPopup = false;

        protected IUIManager uiManager;
        private IEventsManager eventsManager;
        private IApp app;

        private IWindowTransition transition;
        private readonly List<Widget> widgets = new();

        public event Action OnShown;
        public event Action OnHidden;
        public event Action OnBeforeShow;
        public event Action OnBeforeHide;

        public bool Activated { get; private set; }
        public bool IsShown => gameObject.activeSelf;
        public bool Initialized { get; private set; }
        public bool IsPopup => isPopup;

        private RectTransform rect;
        private Action hiddenCallback;
        private Action beforeHideCallback;

        void IWindow.Init(IApp app)
        {
            if (Initialized)
                return;

            this.app = app;
            eventsManager = app.EventsManager;
            uiManager = app.UIManager;

            if (TryGetComponent(out IWindowTransition t))
            {
                transition = t;
                transition.Init();
            }

            rect = GetComponent<RectTransform>();

#if UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
            if (ignoreSafeArea == false)
                ApplySafeArea();
#endif
            GetComponentsInChildren(true, widgets);

            foreach (var widget in widgets)
            {
                widget.Init(app, this);
            }

            if (closeButton)
                closeButton.OnClick += Hide;

            OnInit(app);

            Initialized = true;
        }

        void IWindow.Dispose()
        {
            OnDispose();

            foreach (var widget in widgets)
            {
                widget.Dispose();
            }
        }

        protected abstract void OnInit(IApp app);

        protected virtual void OnDispose()
        {
        }

        public void Show()
        {
            Show(null);
        }

        public void ShowImmediately()
        {
            ShowImmediately(null);
        }

        public void ShowImmediately(Action done, Action onHidden = null, Action onBeforeHide = null)
        {
            gameObject.SetActive(true);
            Activated = true;
            OnBeforeShow?.Invoke();
            BeforeShown();
            hiddenCallback += onHidden;
            beforeHideCallback += onBeforeHide;
            Shown();
            OnShown?.Invoke();
            done?.Invoke();
            eventsManager.Invoke(new CoreEvents.WindowOpenedEvent(this));
        }

        public void Show(Action done, Action onHidden = null, Action onBeforeHide = null)
        {
            gameObject.SetActive(true);
            Activated = true;

            OnBeforeShow?.Invoke();
            BeforeShown();

            hiddenCallback += onHidden;
            beforeHideCallback += onBeforeHide;

            if (transition != null)
            {
                transition.ShowTransition(() =>
                {
                    Shown();
                    OnShown?.Invoke();

                    done?.Invoke();
                });
            }
            else
            {
                Shown();
                OnShown?.Invoke();

                done?.Invoke();
            }

            eventsManager.Invoke(new CoreEvents.WindowOpenedEvent(this));
        }

        public void ForwardTo<T>(bool waitUntilHidden = true) where T : IWindow
        {
            var window = uiManager.GetWindow<T>();

            if (waitUntilHidden)
            {
                Hide(() => { window.Show(null, onHidden: Show); });
            }
            else
            {
                Hide();
                window.Show(null, onBeforeHide: Show);
            }
        }

        public void Hide()
        {
            Hide(null);
        }

        public void Hide(Action done)
        {
            Activated = false;
            OnBeforeHide?.Invoke();
            BeforeHidden();

            beforeHideCallback?.Invoke();
            beforeHideCallback = null;

            if (transition != null)
            {
                transition.HideTransition(() => { CompleteHideWindow(done); });
            }
            else
            {
                CompleteHideWindow(done);
            }

            eventsManager.Invoke(new CoreEvents.OnWindowClosedEvent(this));
        }

        private void CompleteHideWindow(Action done = null)
        {
            gameObject.SetActive(false);
            Hidden();
            OnHidden?.Invoke();

            hiddenCallback?.Invoke();
            hiddenCallback = null;

            done?.Invoke();
        }

        public void HideImmediately()
        {
            Activated = false;
            OnBeforeHide?.Invoke();
            BeforeHidden();
            beforeHideCallback?.Invoke();
            beforeHideCallback = null;
            CompleteHideWindow();
            eventsManager.Invoke(new CoreEvents.OnWindowClosedEvent(this));
        }

        public virtual void Refresh()
        {
        }

        public void RegisterWidget(Widget widget)
        {
            widget.Init(app, this);
            widgets.Add(widget);
        }

        public void ClearHideCallbacks()
        {
            hiddenCallback = null;
            beforeHideCallback = null;
        }

        public T GetWidget<T>() where T : Widget
        {
            foreach (var w in widgets)
            {
                if (w is T widget)
                {
                    return widget;
                }
            }

            Debug.LogError($"Widget {nameof(T)} not found");
            return default;
        }

        protected virtual void BeforeShown()
        {
        }

        protected virtual void BeforeHidden()
        {
        }

        protected virtual void Shown()
        {
        }

        protected virtual void Hidden()
        {
        }

#if UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
        private void ApplySafeArea()
        {
            var rootCanvas = GetComponentInParent<Canvas>();

            Rect safeArea = Screen.safeArea;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= rootCanvas.pixelRect.width;
            anchorMin.y /= rootCanvas.pixelRect.height;
            anchorMax.x /= rootCanvas.pixelRect.width;
            anchorMax.y /= rootCanvas.pixelRect.height;

            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
        }
#endif
    }
}