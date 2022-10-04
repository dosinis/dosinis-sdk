using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;

#else
using UnityEngine.Experimental.UIElements;
#endif

namespace DosinisSDK.Editor
{
    [InitializeOnLoad]
    public static class ToolbarScenes
    {
        private static ScriptableObject toolbar;
        private static string[] scenePaths;
        private static string[] sceneNames;

        static ToolbarScenes()
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            if (toolbar == null)
            {
                Assembly editorAssembly = typeof(UnityEditor.Editor).Assembly;

                UnityEngine.Object[] toolbars = Resources.FindObjectsOfTypeAll(editorAssembly.GetType("UnityEditor.Toolbar"));
                
                toolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;

                if (toolbar != null)
                {
#if UNITY_2020_1_OR_NEWER
                    var windowBackendPropertyInfo = editorAssembly.GetType("UnityEditor.GUIView")
                        .GetProperty("windowBackend",
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var windowBackend = windowBackendPropertyInfo.GetValue(toolbar);
                    var visualTreePropertyInfo = windowBackend.GetType()
                        .GetProperty("visualTree", BindingFlags.Public | BindingFlags.Instance);
                    var visualTree = (VisualElement)visualTreePropertyInfo.GetValue(windowBackend);
#else
          PropertyInfo  visualTreePropertyInfo =
 editorAssembly.GetType("UnityEditor.GUIView").GetProperty("visualTree", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
          VisualElement visualTree = (VisualElement)visualTreePropertyInfo.GetValue(_toolbar, null);
#endif

                    IMGUIContainer container = (IMGUIContainer)visualTree[0];

                    FieldInfo onGUIHandlerFieldInfo = typeof(IMGUIContainer).GetField("m_OnGUIHandler",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    Action handler = (Action)onGUIHandlerFieldInfo.GetValue(container);

                    handler -= OnGUI;
                    handler += OnGUI;
                    
                    onGUIHandlerFieldInfo.SetValue(container, handler);
                }
            }

            if (scenePaths == null || scenePaths.Length != EditorBuildSettings.scenes.Length)
            {
                var paths = new List<string>();
                var names = new List<string>();

                foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                {
                    if (scene.path == null || scene.path.StartsWith("Assets") == false)
                        continue;

                    string scenePath = Application.dataPath + scene.path.Substring(6);

                    paths.Add(scenePath);
                    names.Add(Path.GetFileNameWithoutExtension(scenePath));
                }

                scenePaths = paths.ToArray();
                sceneNames = names.ToArray();
            }
        }

        private static void OnGUI()
        {
            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                Rect rect = new Rect(0, 0, Screen.width, Screen.height);
                rect.xMin = EditorGUIUtility.currentViewWidth * 0.55f;
                rect.xMax = EditorGUIUtility.currentViewWidth - 350.0f;
                rect.y = 7.0f;

                using (new GUILayout.AreaScope(rect))
                {
                    string sceneName = EditorSceneManager.GetActiveScene().name;
                    int sceneIndex = -1;

                    for (int i = 0; i < sceneNames.Length; ++i)
                    {
                        if (sceneName == sceneNames[i])
                        {
                            sceneIndex = i;
                            break;
                        }
                    }

                    int newSceneIndex = EditorGUILayout.Popup(sceneIndex, sceneNames, GUILayout.Width(200.0f));

                    if (newSceneIndex != sceneIndex)
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        {
                            EditorSceneManager.OpenScene(scenePaths[newSceneIndex], OpenSceneMode.Single);
                        }
                    }
                }
            }
        }
    }
}