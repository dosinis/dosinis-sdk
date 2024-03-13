using System;
using System.Reflection;
using DosinisSDK.Utils;
using UnityEditor;
using UnityEngine;

namespace DosinisSDK.Editor
{
    public static class EditorHelper
    {
        public static Type GetPropertyDrawerType(Type classType)
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
                    
                var drawerType = GetPropertyDrawerType(fieldType);

                if (drawerType == null)
                {
                    Debug.LogWarning($"Cannot find drawer for: {fieldType} type. Using default drawer");
                    return false;
                }
                
                var guiMethod = drawerType.GetMethod("OnGUI", BindingFlags.Instance | BindingFlags.Public);

                if (guiMethod == null)
                {
                    Debug.LogWarning($"No OnGUI method found for {drawerType}. Shouldn't be possible");
                    return false;
                }

                var drawer = (PropertyDrawer)Activator.CreateInstance(drawerType);
                
                offsetPosition.size = new Vector2(offsetPosition.size.x, offsetPosition.size.y / 2);
                
                guiMethod.Invoke(drawer, new object[] { offsetPosition, property, new GUIContent(property.displayName) });
                return true;
            }

            Debug.LogWarning("Cannot find field: " + property.propertyPath + " in " + parentType + " type. Using default drawer");
            return false;
        }
    }
}