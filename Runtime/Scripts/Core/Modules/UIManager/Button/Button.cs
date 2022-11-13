using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DosinisSDK.Core
{
    [RequireComponent(typeof(Image))]
    public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
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
        
        private IButtonAnimation buttonAnimation;
        private ScrollRect scrollRectParent;
        private Image image;
        protected bool heldDown = false;
        protected bool mouseOverObject = false;

        // Events

        public event Action OnClick;

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
            
            buttonAnimation?.PressAnimation();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (interactable == false)
                return;
            
            buttonAnimation?.ReleaseAnimation();

            if (mouseOverObject && heldDown)
            {
                ClickPerformed();
            }

            heldDown = false;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (interactable == false)
                return;

            mouseOverObject = false;
            heldDown = false;
            
            buttonAnimation?.ReleaseAnimation();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (interactable == false)
                return;

            mouseOverObject = true;
        }
        
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (scrollRectParent != null)
            {
                scrollRectParent.OnBeginDrag(eventData);
            }
        }
        
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (scrollRectParent != null)
            {
                scrollRectParent.OnDrag(eventData);
            }
        }
        
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (scrollRectParent != null)
            {
                scrollRectParent.OnEndDrag(eventData);
                OnPointerExit(eventData);
            }
        }

        protected virtual void Awake()
        {
            buttonAnimation = GetComponent<IButtonAnimation>();
            image = GetComponent<Image>();
            
            buttonAnimation?.Init();

            scrollRectParent = GetComponentInParent<ScrollRect>();
            
            buttonAnimation?.OnInteractableStateChanged(interactable);
        }

        private void OnEnable()
        {
            buttonAnimation?.ReleaseAnimation();
        }
    }
}