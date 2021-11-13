using DosinisSDK.UI.Utils;
using UnityEditor;
using UnityEngine;

namespace DosinisSDK.Editor
{
    [CustomEditor(typeof(BouncyButton))]
    public class BouncyButtonEditor : UnityEditor.UI.ButtonEditor
    {
        private BouncyButton button;

        protected override void OnEnable()
        {
            base.OnEnable();

            button = serializedObject.targetObject as BouncyButton;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(10);
            button.scaleRatio = EditorGUILayout.FloatField("Scale Ratio", button.scaleRatio);
            button.animationDuration = EditorGUILayout.FloatField("Animation Duration", button.animationDuration);
            button.clickSfx = EditorGUILayout.ObjectField("Click SFX", button.clickSfx, typeof(AudioClip), false) as AudioClip;
        }
    }
}
