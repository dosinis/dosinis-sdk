using DosinisSDK.Utils;
using UnityEditor;
using UnityEngine;

namespace DosinisSDK.Editor
{
    [CustomPropertyDrawer(typeof(FloatRange))]
    public class FloatRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			
            EditorGUI.indentLevel++;
			
            var levelRect = new Rect(position.x - 15, position.y, 100, position.height);
            var valueRect = new Rect(position.x + 75, position.y, 100, position.height);
			
            EditorGUI.PropertyField(levelRect, property.FindPropertyRelative("min"), GUIContent.none);
            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("max"), GUIContent.none);
            
            EditorGUI.indentLevel--;
			
            EditorGUI.EndProperty();
        }
    }
}