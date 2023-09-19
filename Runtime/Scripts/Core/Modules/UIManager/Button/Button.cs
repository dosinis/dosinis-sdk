using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DosinisSDK.Core
{
    [RequireComponent(typeof(Image))]
    public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler
    {
        // Properties
        
        public Image Image => image ? image : GetComponent<Image>();

        public bool Interactable
        {
            get => interactable;
            set
            {
                buttonAnimation?.OnInteractableStateChanged(value);
                interactable = value;
            }
        }

        // Serialized
        
        [SerializeField] protected bool interactable = true;

        // Private
        
        protected IButtonAnimation buttonAnimation;
        private ScrollRect scrollRectParent;
        private Image image;
        protected bool heldDown = false;
        protected bool mouseOverObject = false;

        // Events

        public event Action OnClick;
        public event Action OnPointerEntered;
        public event Action OnPointerExited;
        public event Action OnPressedIn;
        public event Action OnReleased;

        // Button

        protected virtual void ClickPerformed()
        {
            if (interactable == false)
                return;

            OnClick?.Invoke();
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (interactable == false)
                return;
            
            heldDown = true;
            OnPressedIn?.Invoke();
            
            buttonAnimation?.PressAnimation();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (interactable == false)
                return;
            
            buttonAnimation?.ReleaseAnimation();

            ClickPerformed();

            heldDown = false;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (interactable == false)
                return;
            
            OnPointerExited?.Invoke();

            mouseOverObject = false;
            heldDown = false;
            
            buttonAnimation?.ReleaseAnimation();
            buttonAnimation?.Highlight(false);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (interactable == false)
                return;
            
            OnPointerEntered?.Invoke();

            mouseOverObject = true;
            
            buttonAnimation?.Highlight(true);
        }
        
        protected virtual void Awake()
        {
            buttonAnimation = GetComponent<IButtonAnimation>();
            image = GetComponent<Image>();
            
            buttonAnimation?.Init();
            buttonAnimation?.OnInteractableStateChanged(interactable);
        }

        private void OnEnable()
        {
            buttonAnimation?.ReleaseAnimation();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            OnReleased?.Invoke();
        }
    }
}