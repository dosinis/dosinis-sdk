using System;
using UnityEngine;

namespace DosinisSDK.Core
{
    public abstract class Window : MonoBehaviour, IWindow
    {
#if UNITY_ANDROID || UNITY_IOS
        [SerializeField] protected bool ignoreSafeArea = false;
#endif
        [SerializeField] protected Button closeButton;

        protected IUIManager uiManager;
        
        private IWindowTransition transition;
        private Widget[] widgets = { };

        public event Action OnShown;
        public event Action OnHidden;
        public event Action OnBeforeShow;
        public event Action OnBeforeHide;
        
        public bool IsShown { get; private set; }
        public bool Initialized { get; private set; }

        private RectTransform rect;
        private Action hiddenCallback;
        private Action beforeHideCallback;

        void IWindow.Init(IApp app)
        {
            if (Initialized)
                return;

            uiManager = app.UIManager;
            
            if (TryGetComponent(out IWindowTransition t))
            {
                transition = t;
                transition.Init();
            }

            rect = GetComponent<RectTransform>();
            
#if UNITY_ANDROID || UNITY_IOS
            if (ignoreSafeArea == false)
                ApplySafeArea();
#endif
            widgets = GetComponentsInChildren<Widget>(true);

            foreach (var widget in widgets)
            {
                widget.Init(app, this);
            }

            if (closeButton) 
                closeButton.OnClick += Hide;

            IsShown = gameObject.activeInHierarchy;
            
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
            Show(() => { });
        }

        public void Show(Action done, Action onHidden = null, Action onBeforeHide = null)
        {
            gameObject.SetActive(true);

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

                    IsShown = true;
                    done?.Invoke();
                });
            }
            else
            {
                Shown();
                OnShown?.Invoke();

                IsShown = true;
                done?.Invoke();
            }           
        }

        public void ForwardTo<T>(bool waitUntilHidden = true) where T : IWindow
        {
            var window = uiManager.GetWindow<T>();
            
            if (waitUntilHidden)
            {
                Hide(() =>
                {
                    window.Show(null, onHidden: Show);
                });
            }
            else
            {
                Hide();
                window.Show(null, onBeforeHide: Show);
            }
        }

        public void Hide()
        {
            Hide(() => { });
        }

        public void Hide(Action done)
        {
            OnBeforeHide?.Invoke();
            BeforeHidden();
            
            beforeHideCallback?.Invoke();
            beforeHideCallback = null;

            if (transition != null)
            {
                transition.HideTransition(() =>
                {
                    gameObject.SetActive(false);
                    Hidden();
                    OnHidden?.Invoke();

                    hiddenCallback?.Invoke();
                    hiddenCallback = null;
                    
                    IsShown = false;
                    done?.Invoke();
                });
            }
            else
            {
                gameObject.SetActive(false);
                Hidden();
                OnHidden?.Invoke();

                hiddenCallback?.Invoke();
                hiddenCallback = null;
                
                IsShown = false;
                done?.Invoke();
            }
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
        
#if UNITY_ANDROID || UNITY_IOS
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

