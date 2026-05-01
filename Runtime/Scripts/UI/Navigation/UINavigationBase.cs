using System;
using DosinisSDK.Core;
using DosinisSDK.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DosinisSDK.UI.Navigation
{
    public class UINavigationBase : ManagedBehaviour, IUINavigationElement
    {
        [SerializeField] protected UINavigationBase moveUp;
        [SerializeField] protected UINavigationBase moveDown;
        [SerializeField] protected UINavigationBase moveLeft;
        [SerializeField] protected UINavigationBase moveRight;

        [Header("Optional")] [SerializeField] protected GameObject target;
        [SerializeField] private bool startNavigationFromHere = false;
        [SerializeField] protected bool isActiveNavigation = true;
        protected bool isSelected;

        protected IUINavigationController navigationController;

        public bool IsEnabled => Target.activeInHierarchy;

        public virtual bool IsActiveNavigation
        {
            get => isActiveNavigation;
            protected set => isActiveNavigation = value;
        }

        public bool StartNavigationFromHere => startNavigationFromHere;
        public virtual GameObject Target => target;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (target == null)
                target = gameObject;
        }
#endif

        protected override void OnInit(IApp app)
        {
            navigationController = app.GetModule<IUINavigationController>();
            if (IsEnabled)
            {
                navigationController.RegisterElement(this);
            }
        }

        protected virtual void OnEnable()
        {
            navigationController?.RegisterElement(this);
        }

        protected virtual void OnDisable()
        {
            navigationController?.UnregisterElement(this);
            isSelected = false;
        }

        protected override void OnDispose()
        {
            navigationController.UnregisterElement(this);
            isSelected = false;
        }

        protected virtual void OnSelect()
        {
            if (!IsActiveNavigation) return;
            EventSystem.current.SetSelectedGameObject(Target);
            ExecuteEvents.Execute(Target, new PointerEventData(EventSystem.current) { pointerId = -1 },
                ExecuteEvents.pointerEnterHandler);
            isSelected = true;
        }

        protected virtual void OnDeselect()
        {
            if (!IsActiveNavigation) return;
            isSelected = false;
            ExecuteEvents.Execute(Target, new PointerEventData(EventSystem.current) { pointerId = -1 },
                ExecuteEvents.pointerExitHandler);
        }

        protected virtual void OnSubmit()
        {
            if (!IsActiveNavigation) return;
            ExecuteEvents.Execute(Target, new PointerEventData(EventSystem.current) { pointerId = -1, },
                ExecuteEvents.pointerClickHandler);
            ExecuteEvents.Execute(Target, new PointerEventData(EventSystem.current) { pointerId = -1, },
                ExecuteEvents.submitHandler);
        }

        protected virtual void OnHold()
        {
            if (!IsActiveNavigation) return;

            ExecuteEvents.Execute(Target, new PointerEventData(EventSystem.current) { pointerId = -1 },
                ExecuteEvents.pointerDownHandler);
        }

        protected virtual void OnUnhold()
        {
            if (!IsActiveNavigation) return;

            ExecuteEvents.Execute(Target, new PointerEventData(EventSystem.current) { pointerId = 1 },
                ExecuteEvents.pointerUpHandler);
        }

        protected virtual void OnMove(Vector2 axis)
        {
            if (!IsActiveNavigation) return;

            if (axis.y > 0.5f && moveUp != null && moveUp.IsActiveNavigation)
            {
                navigationController.SetCurrentElement(moveUp);
            }
            else if (axis.y < -0.5f && moveDown != null && moveDown.IsActiveNavigation)
            {
                navigationController.SetCurrentElement(moveDown);
            }
            else if (axis.x < -0.5f && moveLeft != null && moveLeft.IsActiveNavigation)
            {
                navigationController.SetCurrentElement(moveLeft);
            }
            else if (axis.x > 0.5f && moveRight != null && moveRight.IsActiveNavigation)
            {
                navigationController.SetCurrentElement(moveRight);
            }
        }

        protected virtual void OnCancel()
        {
        }


        public void SetStartNavigationFromHere(bool value)
        {
            startNavigationFromHere = value;
            if (startNavigationFromHere)
            {
                navigationController?.SetCurrentElement(this);
            }
        }


        public void SetNavigationElement(NavigationDirection direction, IUINavigationElement element)
        {
            if (element is not UINavigationBase navigationBase) return;
            switch (direction)
            {
                case NavigationDirection.Up:
                    moveUp = navigationBase;
                    break;
                case NavigationDirection.Left:
                    moveLeft = navigationBase;
                    break;
                case NavigationDirection.Right:
                    moveRight = navigationBase;
                    break;
                case NavigationDirection.Down:
                    moveDown = navigationBase;
                    break;
            }
        }


        public void Select()
        {
            OnSelect();
        }

        public void Deselect()
        {
            OnDeselect();
        }

        public void Submit()
        {
            OnSubmit();
        }

        public void Hold()
        {
            OnHold();
        }

        public void SetActiveNavigation(bool value)
        {
            IsActiveNavigation = value;
        }

        public void Unhold()
        {
            OnUnhold();
        }

        public void Cancel()
        {
            OnCancel();
        }

        public void Move(Vector2 axis)
        {
            OnMove(axis);
        }

        public void Dispose()
        {
            OnDispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}