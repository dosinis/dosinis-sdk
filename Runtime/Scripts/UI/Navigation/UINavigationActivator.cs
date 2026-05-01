using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    /// <summary>
    /// For world UI purposes only, when you change camera mode to UI 
    /// </summary>
    public class UINavigationActivator : MonoBehaviour
    {
        [SerializeField] private List<UINavigationBase> navigationElements;

#if UNITY_EDITOR
        private void OnValidate()
        {
            var elements = GetComponentsInChildren<UINavigationBase>();
            if (elements == null) return;
            navigationElements = elements.ToList();
        }

        [ContextMenu("Disable Navigation")]
        private void DisableAllElementsNavigation()
        {
            if (navigationElements == null) return;
            foreach (var element in navigationElements)
            {
                element.SetActiveNavigation(false);
            }
        }
#endif
        public void SetActiveNavigation(bool active)
        {
            foreach (var element in navigationElements)
            {
                element.SetActiveNavigation(active);
                if (active && element.StartNavigationFromHere)
                {
                    element.SetStartNavigationFromHere(true);
                }
            }
        }
    }
}