using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DosinisSDK.Core
{
    public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler, IDragHandler
    {
        public Image Image => image ? image : GetComponent<Image>();

        public bool interactable = true;

        private IButtonAnimation buttonAnimation;

        private bool heldDown = false;
        private bool mouseOverObject = false;
        private Image image;

        // Events

        public event Action OnClick;

        // Button

        protected virtual void ClickPerformed()
        {
            if (interactable == false)
                return;

            OnClick?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (interactable == false)
                return;
            
            heldDown = true;

            if (buttonAnimation != null)
            {
                buttonAnimation.PressAnimation();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (interactable == false)
                return;

            if (buttonAnimation != null)
            {
                buttonAnimation.ReleaseAnimation();
            }

            if (mouseOverObject && heldDown)
            {
                ClickPerformed();
            }

            heldDown = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (interactable == false)
                return;

            mouseOverObject = false;
            heldDown = false;

            if (buttonAnimation != null)
            {
                buttonAnimation.ReleaseAnimation();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (interactable == false)
                return;

            mouseOverObject = true;
        }
        public void OnDrag(PointerEventData eventData)
        {
            // Needed, because OnPointerUp doesn't work well without this
        }

        protected virtual void Awake()
        {
            buttonAnimation = GetComponent<IButtonAnimation>();
            image = GetComponent<Image>();

            if (buttonAnimation != null)
            {
                buttonAnimation.Init();
            }
        }

        private void OnEnable()
        {
            if (buttonAnimation != null)
                buttonAnimation.ReleaseAnimation();
        }
    }
}