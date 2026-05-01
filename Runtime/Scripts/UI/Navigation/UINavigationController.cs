using System;
using System.Collections.Generic;
using DosinisSDK.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace DosinisSDK.UI.Navigation
{
    public class UINavigationController : Module, IUINavigationController
    {
        private UINavigationConfig config;
        private IUINavigationElement currentElement;
        private IUINavigationCancel currentCancellationElement;
        private readonly List<IUINavigationCancel> cancellationElements = new();
        private readonly List<IUINavigationElement> navigationElements = new();
        public event Action<IUINavigationElement> OnCurrentElementChanged;
        public event Action<bool> OnTabMovePerformed; 
        


        protected override void OnInit(IApp app)
        {
            config = GetConfigAs<UINavigationConfig>();
            config.OnMoveAction.action.Enable();
            config.OnSubmitAction.action.Enable();
            config.OnCancelAction.action.Enable();
            config.OnTabMovePrevAction.action.Enable();
            config.OnTabMoveNextAction.action.Enable();

            config.OnMoveAction.action.performed += OnMovePerformed;
            config.OnSubmitAction.action.performed += OnSubmitPerformed;
            config.OnSubmitAction.action.canceled += OnSubmitCanceled;
            config.OnCancelAction.action.performed += OnCancelPerformed;
            config.OnCancelAction.action.canceled += OnCancelCanceled;
            config.OnTabMovePrevAction.action.performed += OnTabMovePerformedPrevious;
            config.OnTabMoveNextAction.action.performed += OnTabMovePerformedNext;
        }

        private void OnTabMovePerformedNext(InputAction.CallbackContext obj)
        {
            OnTabMovePerformedCall(obj, true);
        }

        private void OnTabMovePerformedPrevious(InputAction.CallbackContext obj)
        {
            OnTabMovePerformedCall(obj, false);
        }

        private void OnTabMovePerformedCall(InputAction.CallbackContext obj, bool increase)
        {
            if (obj.interaction is PressInteraction)
            {
                OnTabMovePerformed?.Invoke(increase);
            }
        }


        protected override void OnDispose()
        {
            config.OnMoveAction.action.performed -= OnMovePerformed;
            config.OnSubmitAction.action.performed -= OnSubmitPerformed;
            config.OnCancelAction.action.performed -= OnCancelPerformed;
            config.OnCancelAction.action.canceled -= OnCancelCanceled;
            config.OnTabMovePrevAction.action.performed -= OnTabMovePerformedPrevious;
            config.OnTabMoveNextAction.action.performed -= OnTabMovePerformedNext;
            config.OnMoveAction.action.Disable();
            config.OnSubmitAction.action.Disable();
            config.OnCancelAction.action.Disable();
            config.OnTabMoveNextAction.action.Disable();
            config.OnTabMovePrevAction.action.Disable();
            navigationElements.Clear();
            currentElement = null;
        }


        public void RegisterElement(IUINavigationElement element)
        {
            if (currentElement is null || element.StartNavigationFromHere)
            {
                SetCurrentElement(element);
            }

            navigationElements.Add(element);
        }

        public void RegisterCancellationElement(IUINavigationCancel element)
        {
            currentCancellationElement = element;
            cancellationElements.Add(element);
        }

        public void UnregisterCancellationElement(IUINavigationCancel element)
        {
            cancellationElements.Remove(element);
            LookForNewCancellationElement();
        }

        private void LookForNewCancellationElement()
        {
            currentCancellationElement = cancellationElements.Count > 0 ? cancellationElements[0] : null;
        }

        public void RebuildNavigation()
        {
            IUINavigationElement elementForStart = null;
            foreach (var element in navigationElements)
            {
                if (element.StartNavigationFromHere)
                {
                    elementForStart = element;
                    break;
                }

                elementForStart ??= element;
            }

            SetCurrentElement(elementForStart);
        }

        public void UnregisterElement(IUINavigationElement element)
        {
            if (navigationElements.Contains(element))
            {
                navigationElements.Remove(element);
                if (element.StartNavigationFromHere)
                {
                    RebuildNavigation();
                }
            }
        }

        public void SetCurrentElement(IUINavigationElement element)
        {
            currentElement?.Deselect();
            currentElement = element;
            currentElement?.Select();
            OnCurrentElementChanged?.Invoke(currentElement);
        }

        private void SyncWithEventSystem()
        {
            var selectedGameObject = EventSystem.current.currentSelectedGameObject;

            if (selectedGameObject is null || currentElement == null ||
                selectedGameObject.Equals(currentElement.Target)) return;

            if (selectedGameObject.TryGetComponent(out IUINavigationElement element))
            {
                SetCurrentElement(element);
            }
        }

        private void OnMovePerformed(InputAction.CallbackContext obj)
        {
            SyncWithEventSystem();
            if (currentElement is null) return;
            var moveInput = obj.ReadValue<Vector2>();
            moveInput.Normalize();
            currentElement.Move(moveInput);
        }

        private void OnSubmitCanceled(InputAction.CallbackContext obj)
        {
            if (currentElement is null) return;
            currentElement.Unhold();
            if (obj.interaction is PressInteraction)
            {
                currentElement.Submit();
            }
        }

        private void OnSubmitPerformed(InputAction.CallbackContext obj)
        {
            SyncWithEventSystem();
            if (currentElement is null) return;
            currentElement.Hold();
        }

        private void OnCancelPerformed(InputAction.CallbackContext obj)
        {
            SyncWithEventSystem();
            if (obj.interaction is PressInteraction)
            {
                currentElement?.Cancel();
                currentCancellationElement?.Cancel();
            }
        }

        private void OnCancelCanceled(InputAction.CallbackContext obj)
        {
        }
    }
}