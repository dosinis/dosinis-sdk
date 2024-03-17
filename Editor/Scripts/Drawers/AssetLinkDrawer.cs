using DosinisSDK.Assets;
using DosinisSDK.Utils;
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

            const int OFFSET = 150;
            
            var labelPos = new Rect(position.x, position.y, 100, position.height);
            var idRect = new Rect(position.x + labelPos.width, position.y, position.width - labelPos.width - OFFSET, position.height);
            var objRect = new Rect(position.x + idRect.width + labelPos.width, position.y, position.width - idRect.width - labelPos.width, position.height);
            
            if (label.text == pathProperty.stringValue)
            {
                label.text = "Path";
            }
            
            EditorGUI.LabelField(labelPos, label);
            
            loadedObj = EditorGUI.ObjectField(objRect, loadedObj, typeof(Object), false);
            
            if (AssetDatabase.GetAssetPath(loadedObj).StartsWith("Assets/Resources/") == false) // && not addressable
            {
                EditorGUI.HelpBox(idRect, $"Asset is invalid (should be in Resources folder or Addressable)", MessageType.Error);
            }
            else
            {
                EditorGUI.PropertyField(idRect, pathProperty, GUIContent.none);
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var pathProperty = property.FindPropertyRelative("path");
            var guidProperty = property.FindPropertyRelative("guid");

            var assetPath = AssetDatabase.GUIDToAssetPath(guidProperty.stringValue);
            var loadedObj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            
            Draw(ref loadedObj, position, property, label);
           
            if (loadedObj == null)
            {
                property.serializedObject.ApplyModifiedProperties();
                return;
            }

            var loadedObjPath = AssetDatabase.GetAssetPath(loadedObj);
            guidProperty.stringValue = AssetDatabase.AssetPathToGUID(loadedObjPath);
            
            // TODO: Implement Addressables support
            pathProperty.stringValue = EditorUtils.GetAssetPathResourcesAdjusted(loadedObjPath);
                
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}