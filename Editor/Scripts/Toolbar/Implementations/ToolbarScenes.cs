using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

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
            ToolbarExtender.rightToolbarGUI.Add(OnToolbarGUI);
        }

        static void OnToolbarGUI()
        {
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