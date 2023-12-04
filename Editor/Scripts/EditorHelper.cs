using System;
using System.Reflection;
using DosinisSDK.Utils;
using UnityEditor;
using UnityEngine;

namespace DosinisSDK.Editor
{
    public static class EditorHelper
    {
        public static Type GetPropertyDrawer(Type classType)
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var scriptAttributeUtility = assembly.CreateInstance("UnityEditor.ScriptAttributeUtility");
            var scriptAttributeUtilityType = scriptAttributeUtility.GetType();
 
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;
            var getDrawerTypeForType = scriptAttributeUtilityType.GetMethod("GetDrawerTypeForType", bindingFlags);
 
            return (Type)getDrawerTypeForType.Invoke(scriptAttributeUtility, new object[] { classType });
        }

        public static bool DrawCustomProperty(SerializedProperty property, Rect position, ref Rect offsetPosition)
        {
            var parentType = property.serializedObject.targetObject.GetType();
            
            var field = Helper.GetFieldInfoWithReflection(property.propertyPath, parentType);
            
            if (field != null)
            {
                var fieldType = field.FieldType;
                    
                var drawer = GetPropertyDrawer(fieldType);
                    
                var staticMethod = drawer.GetMethod("Draw", BindingFlags.Static | BindingFlags.Public);

                if (staticMethod == null) 
                    return false;
                
                var propLabel = new GUIContent(property?.displayName);
                float childHeight = EditorGUI.GetPropertyHeight(property, propLabel);
                offsetPosition.height = childHeight;
                        
                staticMethod.Invoke(null, new object[] { position, property, propLabel });
                return true;
            }

            Debug.LogWarning("Cannot find field: " + property.propertyPath + " in " + parentType + " type. Using default drawer");
            return false;
        }
    }
}