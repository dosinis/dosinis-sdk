using System;
using DosinisSDK.Core;
using DosinisSDK.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DosinisSDK.UI.Navigation
{
    public class UINavigationBase : ManagedBehaviour, IUINavigationElement
    {
        [SerializeField] private UINavigationBase moveUp;
        [SerializeField] private UINavigationBase moveDown;
        [SerializeField] private UINavigationBase moveLeft;
        [SerializeField] private UINavigationBase moveRight;

        [Header("Optional")] [SerializeField] protected GameObject target;
        [SerializeField] private bool startNavigationFromHere = false;

        protected IUINavigationController navigationController;
        
        public bool IsEnabled => Target.activeInHierarchy;
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
            if (gameObject.activeInHierarchy)
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
        }

        protected virtual void OnDispose()
        {
            navigationController.UnregisterElement(this);
        }

        protected virtual void OnSelect()
        {
            EventSystem.current.SetSelectedGameObject(Target);
            ExecuteEvents.Execute(Target, new PointerEventData(EventSystem.current) { pointerId = -1 },
                ExecuteEvents.pointerEnterHandler);
        }

        protected virtual void OnDeselect()
        {
            ExecuteEvents.Execute(Target, new PointerEventData(EventSystem.current) { pointerId = -1 },
                ExecuteEvents.pointerExitHandler);
        }

        protected virtual void OnSubmit()
        {
            ExecuteEvents.Execute(Target, new PointerEventData(EventSystem.current) { pointerId = -1 },
                ExecuteEvents.pointerClickHandler);
        }

        protected virtual void OnHold()
        {
            ExecuteEvents.Execute(Target, new PointerEventData(EventSystem.current) { pointerId = -1 },
                ExecuteEvents.pointerDownHandler);
        }

        protected virtual void OnUnhold()
        {
            ExecuteEvents.Execute(Target, new PointerEventData(EventSystem.current) { pointerId = 1 },
                ExecuteEvents.pointerUpHandler);
        }

        protected virtual void OnMove(Vector2 axis)
        {
            if (axis.y > 0.5f && moveUp != null)
            {
                navigationController.SetCurrentElement(moveUp);
            }
            else if (axis.y < -0.5f && moveDown != null)
            {
                navigationController.SetCurrentElement(moveDown);
            }
            else if (axis.x < -0.5f && moveLeft != null)
            {
                navigationController.SetCurrentElement(moveLeft);
            }
            else if (axis.x > 0.5f && moveRight != null)
            {
                navigationController.SetCurrentElement(moveRight);
            }
        }

        protected virtual void OnCancel()
        {
        }

        private void OnDestroy()
        {
            Dispose();
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

        public void SetStartNavigationFromHere(bool value)
        {
            startNavigationFromHere = value;
            navigationController?.RebuildNavigation();
        }


        public void SetNavigationElement(NavigationDirection direction, UINavigationBase element)
        {
            switch (direction)
            {
                case NavigationDirection.Up:
                    moveUp = element;
                    break;
                case NavigationDirection.Left:
                    moveLeft = element;
                    break;
                case NavigationDirection.Right:
                    moveRight = element;
                    break;
                case NavigationDirection.Down:
                    moveDown = element;
                    break;
            }
        }

        public void Dispose()
        {
            OnDispose();
        }
    }
}