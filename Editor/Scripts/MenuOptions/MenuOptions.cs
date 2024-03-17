using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DosinisSDK.Assets;
using DosinisSDK.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DosinisSDK.Editor
{
    internal static class MenuOptions
    {
        [MenuItem("DosinisSDK/Validate Asset Links")]
        private static void ValidateAssetLinks()
        {
            var startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            
            Debug.Log("Starting...");
            var scriptableObjects = EditorUtils.GetAssetsOfType<ScriptableObject>();

            foreach (var so in scriptableObjects)
            {
                var type = so.GetType();
                setupAssets(type, so);   
            }
            
            Debug.Log("Finished scriptable Objects. Starting prefabs...");
            AssetDatabase.SaveAssets();
            
            var prefabs = AssetDatabase.FindAssets("t:prefab");
            
            foreach (var guid in prefabs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                var components = go.GetComponents<Component>();
            
                foreach (var component in components)
                {
                    var type = component.GetType();
                    setupAssets(type, component);
                }
            }

            AssetLink refreshLink(AssetLink assetLink)
            {
                var guid = assetLink.guid;
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var loadedObj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

                return new AssetLink(loadedObj);
            }

            void setup(FieldInfo[] fields, object obj, ref bool hasAssetLink, int nestedLevel)
            {
                foreach (var field in fields)
                {
                    if (field.FieldType.GetInterfaces().Contains(typeof(IList<AssetLink>)))
                    {
                        var elements = field.GetValue(obj) as IList<AssetLink>;
                        
                        var foundAssets = new List<AssetLink>();
                        
                        foreach (var link in elements)
                        {
                            hasAssetLink = true;
                            foundAssets.Add(refreshLink(link));
                        }

                        for (int i = 0; i < foundAssets.Count; i++)
                        {
                            elements[i] = foundAssets[i];
                            field.SetValue(obj, elements);
                        }
                    }
                    else if (field.FieldType == typeof(AssetLink))
                    {
                        hasAssetLink = true;
                        field.SetValue(obj, refreshLink((AssetLink)field.GetValue(obj)));
                    }
                    else // check for AssetLink in nested fields (3 levels deep)
                    {
                        // if field is IList<SomeClass> and SomeClass has AssetLink nested
                        if (field.FieldType.GetInterfaces().Contains(typeof(IList)))
                        {
                            var elements = field.GetValue(obj) as IList;
                            
                            if (elements == null)
                                continue;

                            foreach (var element in elements)
                            {
                                if (element == null)
                                    continue;
                                
                                setup(element.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy), element, 
                                    ref hasAssetLink, nestedLevel);
                            }
                        }
                        else // if it's simple field of SomeClass that contains AssetList as nested field
                        {
                            var nestedFields = field.FieldType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                            if (nestedLevel < 3)
                            {
                                var target = field.GetValue(obj);
                                
                                if (target == null)
                                    continue;
                                
                                setup(nestedFields, field.GetValue(obj), ref hasAssetLink, ++nestedLevel);
                            }
                        }
                    }
                }
            }

            void setupAssets(Type type, Object obj)
            {
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                            BindingFlags.DeclaredOnly);
                bool hasAssetLink = false;
                int nestedLevel = 0;
                
                setup(fields, obj, ref hasAssetLink, nestedLevel);
                
                if (hasAssetLink)
                {
                    EditorUtility.SetDirty(obj);
                }
            }
            
            AssetDatabase.SaveAssets();
            Debug.Log($"Finished in {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startTime} ms.");
        }
    }
}