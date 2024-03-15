using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace DosinisSDK.Utils
{
    public class EditorUtils
    {
        /// <summary>
        /// Get all assets of type T
        /// </summary>
        /// <typeparam name="T">type of the asset</typeparam>
        /// <returns></returns>
        public static T[] GetAssetsOfType<T>() where T : Object
        {
            var assets = AssetDatabase.FindAssets($"t:{typeof(T).Name}")
                .Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid))).ToArray();

            return assets;
        }
        
        /// <summary>
        /// Set array of assets of type T to the array and save the changes
        /// </summary>
        /// <param name="array">ref array</param>
        /// <param name="targetObj">target object that will be set dirty</param>
        public static void GetAssetsOfType<T>(ref T[] array, Object targetObj) where T : Object
        {
            array = GetAssetsOfType<T>();
            
            EditorUtility.SetDirty(targetObj);
            AssetDatabase.SaveAssets();
        }

        public static string GetAssetPath(Object obj)
        {
            var path = UnityEditor.AssetDatabase.GetAssetPath(obj);
            
            if (path.StartsWith("Assets/Resources/"))
            {
                path = path.Replace("Assets/Resources/", "").Replace(".asset", "");
            }

            return path;
        }
    }
}
#endif