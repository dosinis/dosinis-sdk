using System;
using System.Reflection;
using DosinisSDK.Inspector;
using DosinisSDK.Utils;
using UnityEditor;
using UnityEngine;

namespace DosinisSDK.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfPropertyDrawer : PropertyDrawer
    {
        private ShowIfAttribute showIf;
        private SerializedProperty comparedProperty;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!Show(property) && showIf.AppearingType == ShowIfAttribute.AppearType.DontDraw)
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }

            if (property.propertyType == SerializedPropertyType.Generic)
            {
                float totalHeight = 0.0f;

                var children = property.GetEnumerator();

                while (children.MoveNext())
                {
                    SerializedProperty child = children.Current as SerializedProperty;

                    GUIContent childLabel = new GUIContent(child?.displayName);

                    totalHeight += EditorGUI.GetPropertyHeight(child, childLabel) +
                                   EditorGUIUtility.standardVerticalSpacing;
                }

                totalHeight -= EditorGUIUtility.standardVerticalSpacing;

                return totalHeight;
            }

            return EditorGUI.GetPropertyHeight(property, label);
        }


        private bool Show(SerializedProperty property)
        {
            showIf = attribute as ShowIfAttribute;

            if (showIf == null)
            {
                throw new NullReferenceException("ShowIfAttribute is null");
            }

            string path = property.propertyPath.Contains(".")
                ? System.IO.Path.ChangeExtension(property.propertyPath, showIf.FieldName)
                : showIf.FieldName;

            comparedProperty = property.serializedObject.FindProperty(path);

            if (comparedProperty == null)
            {
                Debug.LogError("Cannot find property with name: " + path);
                return true;
            }

            if (showIf.ComparedValue == null && comparedProperty.objectReferenceValue == null)
            {
                return true;
            }

            if (showIf.ComparedValue == null && comparedProperty.objectReferenceValue != null)
            {
                return false;
            }

            switch (comparedProperty.type)
            {
                case "bool":
                    return comparedProperty.boolValue.Equals(showIf.ComparedValue);
                case "Enum":
                    return comparedProperty.enumValueIndex.Equals((int)showIf.ComparedValue);
                default:
                    Debug.LogError("Error: " + comparedProperty.type + " is not supported of " + path);
                    return true;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Show(property))
            {
                if (property.propertyType == SerializedPropertyType.Generic)
                {
                    Rect offsetPosition = position;
                    
                    if (EditorHelper.DrawCustomProperty(property, position, ref offsetPosition)) 
                        return;
                   
                    var children = property.GetEnumerator();
                    
                    while (children.MoveNext())
                    {
                        SerializedProperty child = children.Current as SerializedProperty;
                        
                        GUIContent childLabel = new GUIContent(child?.displayName);

                        float childHeight = EditorGUI.GetPropertyHeight(child, childLabel);
                        offsetPosition.height = childHeight;
                        
                        EditorGUI.PropertyField(offsetPosition, child, childLabel);
                        
                        offsetPosition.y += childHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
                else
                {
                    EditorGUI.PropertyField(position, property, label);
                }
            }
            else if (showIf.AppearingType == ShowIfAttribute.AppearType.ReadOnly)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label);
                GUI.enabled = true;
            }
        }
    }
}