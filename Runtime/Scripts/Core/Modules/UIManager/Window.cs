using System;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class Window : MonoBehaviour, IWindow
    {
        [SerializeField] protected bool ignoreSafeArea = false;
        [SerializeField] protected Button closeButton;

        private IWindowTransition transition;
        private Widget[] widgets = { };

        public event Action OnShown;
        public event Action OnHidden;
        public event Action OnBeforeShow;
        public event Action OnBeforeHide;
        
        public bool IsShown { get; private set; }
        public bool Initialized { get; private set; }

        private RectTransform rect;
        private Action hideCallback;
        protected IApp app;

        void IWindow.Init(IApp app)
        {
            if (Initialized)
                return;
            
            this.app = app;
            
            if (TryGetComponent(out IWindowTransition t))
            {
                transition = t;
                transition.Init();
            }

            rect = GetComponent<RectTransform>();
            
            if (ignoreSafeArea == false)
                ApplySafeArea();

            widgets = GetComponentsInChildren<Widget>(true);

            foreach (var widget in widgets)
            {
                widget.Init(this);
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

        protected virtual void OnInit(IApp app)
        {
        }

        protected virtual void OnDispose()
        {
        }

        public void Show()
        {
            Show(() => { });
        }

        public void Show(Action done, Action onHidden = null)
        {
            gameObject.SetActive(true);

            OnBeforeShow?.Invoke();
            BeforeShown();

            hideCallback += onHidden;

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

        public void Hide()
        {
            Hide(() => { });
        }

        public void Hide(Action done)
        {
            hideCallback?.Invoke();
            hideCallback = null;

            OnBeforeHide?.Invoke();
            BeforeHidden();

            if (transition != null)
            {
                transition.HideTransition(() =>
                {
                    gameObject.SetActive(false);
                    Hidden();
                    OnHidden?.Invoke();

                    IsShown = false;
                    done?.Invoke();
                });
            }
            else
            {
                gameObject.SetActive(false);
                Hidden();
                OnHidden?.Invoke();

                IsShown = false;
                done?.Invoke();
            }
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

        // NOTE: This might apply safe area wrongly in Game view (it works on DeviceSimulator though)
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
    }
}

