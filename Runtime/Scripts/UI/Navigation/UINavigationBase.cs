using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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

        protected IUINavigationController navigationController;
        protected CancellationTokenSource cts = new();

        public Observable<bool> IsSelected { get; } = new(false);
        public bool IsEnabled => Target.activeInHierarchy;

        public virtual bool IsActiveNavigation =>
            Target.TryGetComponent(out IInteractableElement interactableElement)
                ? interactableElement.Interactable && isActiveNavigation
                : isActiveNavigation;

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
            IsSelected.Value = false;
        }

        protected override void OnDispose()
        {
            navigationController?.UnregisterElement(this);
            IsSelected.Value = false;
        }

        protected virtual void OnSelect()
        {
            if (!IsActiveNavigation) return;
            EventSystem.current.SetSelectedGameObject(Target);
            ExecuteEvents.Execute(Target, new PointerEventData(EventSystem.current) { pointerId = -1 },
                ExecuteEvents.pointerEnterHandler);
            IsSelected.Value = true;
        }

        protected virtual void OnDeselect()
        {
            if (Target == null || !IsActiveNavigation) return;
            IsSelected.Value = false;
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
            if (axis.y > 0.5f && moveUp is { IsActiveNavigation: true, isActiveAndEnabled: true })
            {
                navigationController.SetCurrentElement(moveUp);
            }
            else if (axis.y < -0.5f && moveDown is { IsActiveNavigation: true, isActiveAndEnabled: true })
            {
                navigationController.SetCurrentElement(moveDown);
            }
            else if (axis.x < -0.5f && moveLeft is { IsActiveNavigation: true, isActiveAndEnabled: true })
            {
                navigationController.SetCurrentElement(moveLeft);
            }
            else if (axis.x > 0.5f && moveRight is { IsActiveNavigation: true, isActiveAndEnabled: true })
            {
                navigationController.SetCurrentElement(moveRight);
            }
        }

        protected virtual void OnCancel()
        {
        }

        protected virtual async UniTask SimulateSubmit()
        {
            OnSelect();
            await UniTask.NextFrame(cts.Token);
            OnHold();
            await UniTask.NextFrame(cts.Token);
            OnUnhold();
            await UniTask.NextFrame(cts.Token);
            OnSubmit();
        }

        public bool TryGetNavigationElement(NavigationDirection direction, out IUINavigationElement element)
        {
            switch (direction)
            {
                case NavigationDirection.Up:
                    element = moveUp;
                    break;
                case NavigationDirection.Left:
                    element = moveLeft;
                    break;
                case NavigationDirection.Right:
                    element = moveRight;
                    break;
                case NavigationDirection.Down:
                    element = moveDown;
                    break;
                default:
                    element = null;
                    break;
            }

            return element != null;
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

        public void SetActiveNavigation(bool value)
        {
            isActiveNavigation = value;
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