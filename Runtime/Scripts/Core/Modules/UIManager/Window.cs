using System;
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
        
        public bool IsShown => gameObject.activeSelf;

        private RectTransform rect;

        public void Init()
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
        
        public void Dispose()
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

        public void Show(Action done)
        {
            gameObject.SetActive(true);

            OnBeforeShow?.Invoke();
            BeforeShown();

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
        }

        public void Hide()
        {
            Hide(() => { });
        }

        public void Hide(Action done)
        {
            OnBeforeHide?.Invoke();
            BeforeHidden();

            if (transition != null)
            {
                transition.HideTransition(() =>
                {
                    gameObject.SetActive(false);
                    Hidden();
                    OnHidden?.Invoke();

                    done?.Invoke();
                });
            }
            else
            {
                gameObject.SetActive(false);
                Hidden();
                OnHidden?.Invoke();

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

