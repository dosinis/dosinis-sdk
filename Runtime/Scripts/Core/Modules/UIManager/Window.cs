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

        public virtual void Init(IUIManager uiManager)
        {
            if (TryGetComponent(out IWindowTransition t))
            {
                transition = t;
                transition.Init();
            }

            rect = GetComponent<RectTransform>();
            
            if (ignoreSafeArea == false)
                ApplySafeArea();
        }

        public void Show(Action done = null)
        {
            gameObject.SetActive(true);

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

        public void Hide(Action done = null)
        {
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

        public virtual void OnShown()
        {
        }

        public virtual void OnHidden()
        {
        }

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

