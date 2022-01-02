using System;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class Window : MonoBehaviour
    {
        [SerializeField] protected bool ignoreSafeArea = false;

        public event Action OnWindowShown;
        public event Action OnWindowHidden;

        public bool IsShown { get; private set; }

        private RectTransform rect;

        public virtual void Init(UIManager uiManager)
        {
            IsShown = gameObject.activeSelf;

            rect = GetComponent<RectTransform>();
            
            if (ignoreSafeArea == false)
                ApplySafeArea();
        }

        public virtual void Show(Action done = null)
        {
            gameObject.SetActive(true);
            IsShown = true;
            OnShown();

            done?.Invoke();
        }

        public virtual void Hide(Action done = null)
        {
            gameObject.SetActive(false);
            IsShown = false;
            OnHidden();

            done?.Invoke();
        }

        public virtual void OnShown()
        {
            OnWindowShown?.Invoke();
        }

        public virtual void OnHidden()
        {
            OnWindowHidden?.Invoke();
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

