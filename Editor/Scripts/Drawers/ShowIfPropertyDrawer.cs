using System;
using DosinisSDK.Inspector;
using UnityEditor;
using UnityEngine;

namespace DosinisSDK.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfPropertyDrawer : PropertyDrawer
    {
        private ShowIfAttribute showIf;
        private SerializedProperty comparedField;

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

            comparedField = property.serializedObject.FindProperty(path);

            if (comparedField == null)
            {
                Debug.LogError("Cannot find property with name: " + path);
                return true;
            }

            switch (comparedField.type)
            {
                case "bool":
                    return comparedField.boolValue.Equals(showIf.ComparedValue);
                case "Enum":
                    return comparedField.enumValueIndex.Equals((int)showIf.ComparedValue);
                default:
                    Debug.LogError("Error: " + comparedField.type + " is not supported of " + path);
                    return true;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Show(property))
            {
                if (property.propertyType == SerializedPropertyType.Generic)
                {
                    var children = property.GetEnumerator();

                    Rect offsetPosition = position;

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