using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

namespace DosinisSDK.Editor
{
    [Overlay(typeof(EditorWindow), "Utility Toolbar")]
    public class UtilityToolbar : Overlay
    {
        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.style.alignItems = Align.Center;

            // ----- DELETE SAVES -----
            var deleteBtn = new Button(() =>
            {
                if (Directory.Exists("Assets/Saves"))
                {
                    AssetDatabase.DeleteAsset("Assets/Saves");
                    AssetDatabase.Refresh();
                }
            });
            deleteBtn.text = "Clear Saves";
            root.Add(deleteBtn);

            // ----- SCENE DROPDOWN -----
            var sceneDropdown = new DropdownField();
            var sceneNames = new System.Collections.Generic.List<string>();

            foreach (var s in EditorBuildSettings.scenes)
                sceneNames.Add(Path.GetFileNameWithoutExtension(s.path));

            sceneDropdown.choices = sceneNames;
            sceneDropdown.RegisterValueChangedCallback(v =>
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    var index = sceneDropdown.index;
                    EditorSceneManager.OpenScene(EditorBuildSettings.scenes[index].path);
                }
            });

            root.Add(sceneDropdown);

            // ----- TIME SCALE -----

            var slider = new Slider(0, 5);
            slider.value = Time.timeScale;
            slider.style.width = 150;

            slider.RegisterValueChangedCallback(v =>
            {
                Time.timeScale = v.newValue;
            });
            
            var resetBtn = new Button(() =>
            {
                slider.value = 1;
                Time.timeScale = 1;
            });
            
            resetBtn.text = "Reset";
            
            root.Add(slider);
            root.Add(resetBtn);
            
            return root;
        }
    }

}
