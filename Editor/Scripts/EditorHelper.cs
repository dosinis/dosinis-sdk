using System;
using System.Reflection;

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
    }
}