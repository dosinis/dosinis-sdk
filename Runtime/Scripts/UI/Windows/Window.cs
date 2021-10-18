using System;
using UnityEngine;
using UnityEngine.UI;

namespace DosinisSDK.UI
{
    public class Window : MonoBehaviour
    {
        [SerializeField] protected Button closeButton;

        public event Action OnWindowShown = () => { };
        public event Action OnWindowHidden = () => { };

        public bool IsShown { get; private set; }

        public virtual void Init(IUIManager uiManager)
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }

            IsShown = gameObject.activeSelf;
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            IsShown = true;
            OnShown();
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            IsShown = false;
            OnHidden();
        }

        public virtual void OnShown()
        {
            OnWindowShown();
        }

        public virtual void OnHidden()
        {
            OnWindowHidden();
        }
    }
}

