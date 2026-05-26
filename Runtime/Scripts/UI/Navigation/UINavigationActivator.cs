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
            if (elements == null) return;
            navigationElements = elements.ToList();
            EditorUtility.SetDirty(this);
        }

        [ContextMenu("Disable Navigation")]
        private void DisableAllElementsNavigation()
        {
            if (navigationElements == null) return;
            foreach (var element in navigationElements)
            {
                element.SetActiveNavigation(false);
                EditorUtility.SetDirty(element);
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
                    controller.SetCurrentElement(element);
                }
            }
        }

        protected override void OnInit(IApp app)
        {
            controller = app.GetModule<IUINavigationController>();
        }
    }
}