using System.Collections.Generic;
using System.Linq;
using DosinisSDK.Core;
using DosinisSDK.Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    /// <summary>
    /// For world UI purposes only, when you change camera mode to UI 
    /// </summary>
    public class UINavigationActivator : ManagedBehaviour
    {
        [SerializeField] private List<UINavigationBase> navigationElements;
        [SerializeField] private List<UIAction> navigationActions;
        private IUINavigationController controller;

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateNavigationElements();
        }

        [ContextMenu("Update Navigation Elements")]
        private void UpdateNavigationElements()
        {
            var elements = GetComponentsInChildren<UINavigationBase>(true);
            if (elements != null)
            {
                navigationElements = elements.ToList();
                EditorUtility.SetDirty(this);
            }

            var actions = GetComponentsInChildren<UIAction>(true);
            if (actions != null)
            {
                navigationActions = actions.ToList();
                EditorUtility.SetDirty(this);
            }
        }

        [ContextMenu("Disable Navigation")]
        private void DisableAllElementsNavigation()
        {
            if (navigationElements != null)
            {
                foreach (var element in navigationElements)
                {
                    element.SetActiveNavigation(false);
                    EditorUtility.SetDirty(element);
                }
            }

            if (navigationActions != null)
            {
                foreach (var action in navigationActions)
                {
                    action.SetActiveNavigation(false);
                    EditorUtility.SetDirty(action);
                }
            }
        }
#endif
        public void SetActiveNavigation(bool active)
        {
            foreach (var element in navigationElements)
            {
                element.SetActiveNavigation(active);
                if (element.IsEnabled && element.TryGetComponent(out UIStartOnActivationTag _))
                {
                    controller?.SetCurrentElement(element);
                }
            }

            foreach (var action in navigationActions)
            {
                action.SetActiveNavigation(active);
            }
        }

        protected override void OnInit(IApp app)
        {
            controller = app.GetModule<IUINavigationController>();
        }
    }
}