using DosinisSDK.Assets;
using UnityEditor;
using UnityEngine;

namespace DosinisSDK.Editor
{
    [CustomPropertyDrawer(typeof(AssetLink))]
    public class AssetLinkDrawer : PropertyDrawer
    {
        private void Draw(ref Object loadedObj, Rect position, SerializedProperty property, GUIContent label)
        {
            var pathProperty = property.FindPropertyRelative("path");
            
            EditorGUI.BeginProperty(position, label, property);
            
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            const int OFFSET = 200;
            
            var idRect = new Rect(position.x, position.y, position.width - OFFSET, position.height);
            var objRect = new Rect(position.x + position.width - OFFSET, position.y, position.width - idRect.width, position.height);

            loadedObj = EditorGUI.ObjectField(objRect, loadedObj, typeof(Object), false);
            EditorGUI.PropertyField(idRect, pathProperty, GUIContent.none);

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var pathProperty = property.FindPropertyRelative("path");

            if (pathProperty == null)
                return;

            var loadedObj = AssetDatabase.LoadAssetAtPath<Object>(pathProperty.stringValue);

            if (loadedObj == null)
            {
                loadedObj = Resources.Load<Object>(pathProperty.stringValue);
            }
            
            Draw(ref loadedObj, position, property, label);
           
            if (loadedObj == null)
            {
                property.serializedObject.ApplyModifiedProperties();
                return;
            }
            
            pathProperty.stringValue = AssetDatabase.GetAssetPath(loadedObj);
                    
            // TODO: Implement Addressables support
            
            if (pathProperty.stringValue.StartsWith("Assets/Resources/"))
            {
                pathProperty.stringValue = pathProperty.stringValue.Replace("Assets/Resources/", "").Replace(".asset", "");
            }
            else
            {
                Debug.LogWarning("Asset should be in Resources folder: " + pathProperty.stringValue);
            }
                
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}