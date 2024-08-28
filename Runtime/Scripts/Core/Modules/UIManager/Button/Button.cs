using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DosinisSDK.Core
{
    [RequireComponent(typeof(Image))]
    public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler, IButton
    {
        // Properties
        
        public Image Image => image ? image : GetComponent<Image>();

        public bool Interactable
        {
            get => interactable;
            set
            {
                if (interactable != value)
                {
                    buttonAnimation?.OnInteractableStateChanged(value);
                }
                
                interactable = value;
            }
        }

        // Serialized
        
        [SerializeField] private bool interactable = true;

        // Private
        
        protected IButtonAnimation buttonAnimation;
        private Image image;
        public bool HeldDown { get; protected set; }
        public bool MouseOverObject { get; protected set; }
        
        // Events

        public event Action OnClick;
        public event Action OnPointerEntered;
        public event Action OnPointerExited;
        public event Action OnPressedIn;
        public event Action OnReleased;

        // Button

        protected virtual void ClickPerformed()
        {
            if (Interactable == false)
                return;

            OnClick?.Invoke();
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (Interactable == false)
                return;
            
            HeldDown = true;
            OnPressedIn?.Invoke();
            
            buttonAnimation?.PressAnimation();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (Interactable == false)
                return;
            
            buttonAnimation?.ReleaseAnimation();

            ClickPerformed();

            HeldDown = false;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (Interactable == false)
                return;
            
            OnPointerExited?.Invoke();

            MouseOverObject = false;
            HeldDown = false;
            
            buttonAnimation?.ReleaseAnimation();
            buttonAnimation?.Highlight(false);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (Interactable == false)
                return;
            
            OnPointerEntered?.Invoke();

            MouseOverObject = true;
            
            buttonAnimation?.Highlight(true);
        }
        
        private void Awake()
        {
            image = GetComponent<Image>();
            
            buttonAnimation = GetComponent<IButtonAnimation>();
            buttonAnimation?.Init();
            buttonAnimation?.OnInteractableStateChanged(Interactable);
        }

        private async void Start()
        {
            await App.Ready();
            OnInit(App.Core);
        }

        protected virtual void OnEnable()
        {
            buttonAnimation?.ReleaseAnimation();
        }

        protected virtual void OnDisable()
        {
            OnReleased?.Invoke();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            OnReleased?.Invoke();
        }
        
        protected virtual void OnInit(IApp app)
        {
        }
    }
}