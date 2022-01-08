using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DosinisSDK.UI
{
    public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
    {
        public bool interactable = true;

        private IButtonAnimation buttonAnimation;

        private bool heldDown = false;
        private bool mouseOverObject = false;

        // Events

        public event Action OnImpressed;
        public event Action OnReleased;
        public event Action OnClick;

        // Button

        private void ClickPerformed()
        {
            OnClick?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (interactable)
            {
                heldDown = true;

                if (buttonAnimation != null)
                {
                    buttonAnimation.PressAnimation(OnImpressed);
                }
                else
                {
                    OnImpressed?.Invoke();
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (interactable)
            {
                if (buttonAnimation != null)
                {
                    buttonAnimation.ReleaseAnimation(OnReleased);
                }
                else
                {
                    OnReleased?.Invoke();
                }

                if (mouseOverObject && heldDown)
                {
                    ClickPerformed();
                }

            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mouseOverObject = false;
            heldDown = false;

            if (interactable && buttonAnimation != null)
            {
                buttonAnimation.ReleaseAnimation(OnReleased);
            }
            else
            {
                OnReleased?.Invoke();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (interactable)
            {
                mouseOverObject = true;
            }
        }

        private void Awake()
        {
            buttonAnimation = GetComponent<IButtonAnimation>();

            if (buttonAnimation != null)
            {
                buttonAnimation.Init();
            }
        }
    }
}