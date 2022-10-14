using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

namespace DosinisSDK.Editor
{
    public static class ToolbarCallback
    {
        private static readonly Type toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static readonly Type guiViewType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GUIView");
#if UNITY_2020_1_OR_NEWER
        private static readonly Type windowBackendType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.IWindowBackend");

        private static readonly PropertyInfo windowBackend = guiViewType.GetProperty("windowBackend", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly PropertyInfo viewVisualTree = windowBackendType.GetProperty("visualTree", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#else
		private static readonly PropertyInfo viewVisualTree = m_guiViewType.GetProperty("visualTree", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif
        private static readonly FieldInfo guiContainerOnGui = typeof(IMGUIContainer).GetField("m_OnGUIHandler", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        private static ScriptableObject currentToolbar;

        public static Action onToolbarGUI;
        public static Action onToolbarGUILeft;
        public static Action onToolbarGUIRight;

        static ToolbarCallback()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            // Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
            if (currentToolbar != null)
                return;
            
            var toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
            currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;

            if (currentToolbar == null) 
                return;
            
#if UNITY_2021_1_OR_NEWER
            var rootField = currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
            var rawRoot = rootField?.GetValue(currentToolbar);
            var root = rawRoot as VisualElement;
                    
            registerCallback("ToolbarZoneLeftAlign", onToolbarGUILeft);
            registerCallback("ToolbarZoneRightAlign", onToolbarGUIRight);

            void registerCallback(string r, Action callback)
            {
                var toolbarZone = root.Q(r);

                var parent = new VisualElement()
                {
                    style =
                    {
                        flexGrow = 1,
                        flexDirection = FlexDirection.Row,
                    }
                };
                        
                var container = new IMGUIContainer();
                container.style.flexGrow = 1;
                container.onGUIHandler += () => { callback?.Invoke(); };
                parent.Add(container);
                toolbarZone.Add(parent);
            }
#else
#if UNITY_2020_1_OR_NEWER
					var backend = windowBackend.GetValue(currentToolbar);
					var visualTree = (VisualElement) viewVisualTree.GetValue(backend, null);
#else
					var visualTree = (VisualElement) viewVisualTree.GetValue(currentToolbar, null);
#endif
					var container = (IMGUIContainer) visualTree[0];

					var handler = (Action) guiContainerOnGui.GetValue(container);
					handler -= OnGUI;
					handler += OnGUI;
					guiContainerOnGui.SetValue(container, handler);
#endif
        }

        private static void OnGUI()
        {
            var handler = onToolbarGUI;
            
            if (handler != null) 
                handler();
        }
    }
}