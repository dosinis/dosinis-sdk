using System;
using DosinisSDK.Inspector;
using UnityEditor;
using UnityEngine;

namespace DosinisSDK.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfNotNullAttribute))]
    public class ShowIfNotNullPropertyDrawer : PropertyDrawer
    {
        private ShowIfNotNullAttribute showIf;
        private SerializedProperty comparedProperty;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!Show(property) && showIf.AppearingType == ShowIfNotNullAttribute.AppearType.DontDraw)
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
            showIf = attribute as ShowIfNotNullAttribute;

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

            if (comparedProperty.objectReferenceValue == null)
            {
                return false;
            }

            return true;
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
            else if (showIf.AppearingType == ShowIfNotNullAttribute.AppearType.ReadOnly)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label);
                GUI.enabled = true;
            }
        }
    }
}