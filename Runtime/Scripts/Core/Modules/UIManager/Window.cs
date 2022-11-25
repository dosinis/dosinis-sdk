using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class Window : MonoBehaviour
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

        private RectTransform rect;

        private readonly Dictionary<Type, Action> hideCallbacks = new Dictionary<Type, Action>();

        internal void Init()
        {
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

            OnInit();
        }
        
        internal void Dispose()
        {
            OnDispose();

            foreach (var widget in widgets)
            {
                widget.Dispose();
            }
        }

        protected virtual void OnInit()
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

            if (hideCallbacks.ContainsKey(GetType()) == false && onHidden != null)
            {
                hideCallbacks.Add(GetType(), onHidden);
            }

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
            if (hideCallbacks.TryGetValue(GetType(), out Action callback))
            {
                callback?.Invoke();
                hideCallbacks.Remove(GetType());
            }
            
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

        // This applies safe area wrongly on Game view (it work on DeviceSimulator though)
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

