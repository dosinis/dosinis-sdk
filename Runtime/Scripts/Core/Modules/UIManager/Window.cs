using System;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class Window : MonoBehaviour
    {
        [SerializeField] protected bool ignoreSafeArea = false;

        private IWindowTransition transition;

        public event Action OnWindowShown;
        public event Action OnWindowHidden;

        public bool IsShown => gameObject.activeSelf;

        private RectTransform rect;

        public void Init(IUIManager uiManager)
        {
            if (TryGetComponent(out IWindowTransition t))
            {
                transition = t;
                transition.Init();
            }

            rect = GetComponent<RectTransform>();
            
            if (ignoreSafeArea == false)
                ApplySafeArea();

            OnInit();
        }
        
        protected virtual void OnInit()
        {
        }

        public void Show()
        {
            Show(() => { });
        }

        public void Show(Action done)
        {
            gameObject.SetActive(true);

            OnBeforeShown();

            if (transition != null)
            {
                transition.ShowTransition(() =>
                {
                    OnShown();
                    OnWindowShown?.Invoke();

                    done?.Invoke();
                });
            }
            else
            {
                OnShown();
                OnWindowShown?.Invoke();

                done?.Invoke();
            }           
        }

        public void Hide()
        {
            Hide(() => { });
        }

        public void Hide(Action done)
        {
            OnBeforeHidden();

            if (transition != null)
            {
                transition.HideTransition(() =>
                {
                    gameObject.SetActive(false);
                    OnHidden();
                    OnWindowHidden?.Invoke();

                    done?.Invoke();
                });
            }
            else
            {
                gameObject.SetActive(false);
                OnHidden();
                OnWindowHidden?.Invoke();

                done?.Invoke();
            }
        }

        protected virtual void OnBeforeShown()
        {
        }

        protected virtual void OnBeforeHidden()
        {
        }

        protected virtual void OnShown()
        {
        }

        protected virtual void OnHidden()
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

