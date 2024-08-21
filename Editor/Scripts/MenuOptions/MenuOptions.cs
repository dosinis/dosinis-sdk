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
            
            const int MAX_NESTED_LEVEL = 3;
            
            var scriptableObjects = EditorUtils.GetAssetsOfType<ScriptableObject>();

            foreach (var so in scriptableObjects)
            {
                var type = so.GetType();
                setupAssets(type, so);   
            }
            
            Debug.Log("Finished scanning ScriptableObject. Starting prefabs...");
            AssetDatabase.SaveAssets();
            
            var prefabs = AssetDatabase.FindAssets("t:prefab");
            
            foreach (var guid in prefabs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                var components = go.GetComponents<Component>();
            
                foreach (var component in components)
                {
                    if (component == null)
                    {
                        Debug.LogWarning($"Found null component on {go.name}", go);
                        continue;
                    }
                    
                    var type = component.GetType();
                    setupAssets(type, component);
                }
            }

            AssetLink refreshLink(AssetLink assetLink)
            {
                var guid = assetLink.Guid;
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var loadedObj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

                return new AssetLink(loadedObj);
            }

            object refreshLinkGeneric(AssetLink assetLink, Type type)
            {
                var guid = assetLink.Guid;
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var loadedObj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                
                return Activator.CreateInstance(type, args: loadedObj);
            }

            void setup(FieldInfo[] fields, object obj, ref bool hasAssetLink, int nestedLevel)
            {
                foreach (var field in fields)
                {
                    var fieldType = field.FieldType;
                    var isGeneric = fieldType.IsGenericType;
                    Type elementType = null;
                    Type genericDef = null;
                    Type[] genericArgs = null;

                    if (isGeneric)
                    {
                        genericDef = fieldType.GetGenericTypeDefinition();
                        genericArgs = fieldType.GetGenericArguments();
                    }

                    if (fieldType.IsArray)
                    {
                        elementType = fieldType.GetElementType();
                    }
                    
                    if (fieldType.GetInterfaces().Contains(typeof(IList<AssetLink>)))
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
                    else if (isGeneric && genericDef == typeof(List<>) 
                                       && genericArgs[0].IsGenericType && genericArgs[0].GetGenericTypeDefinition() == typeof(AssetLink<>))
                    {
                        var genericType = fieldType.GetGenericArguments()[0];
                        setupGenericCollection(ref hasAssetLink, genericType);
                    }
                    else if (fieldType.IsArray && elementType != null && elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof(AssetLink<>))
                    {
                        setupGenericCollection(ref hasAssetLink, elementType);
                    }
                    else if (fieldType == typeof(AssetLink))
                    {
                        hasAssetLink = true;
                        field.SetValue(obj, refreshLink((AssetLink)field.GetValue(obj)));
                    }
                    else if (isGeneric && genericDef == typeof(AssetLink<>))
                    {
                        hasAssetLink = true;
                        field.SetValue(obj, refreshLinkGeneric((AssetLink)field.GetValue(obj), fieldType));
                    }
                    else // check for AssetLink in nested fields (3 levels deep)
                    {  
                        // if field is IList<SomeClass> and SomeClass has AssetLink nested
                        if (fieldType.GetInterfaces().Contains(typeof(IList)))
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
                            var nestedFields = fieldType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                            if (nestedLevel < MAX_NESTED_LEVEL)
                            {
                                var target = field.GetValue(obj);
                                
                                if (target == null)
                                    continue;
                                
                                setup(nestedFields, field.GetValue(obj), ref hasAssetLink, ++nestedLevel);
                            }
                        }
                    }
                    
                    void setupGenericCollection(ref bool hasAssetLink, Type linkType)
                    {
                        var elements = field.GetValue(obj) as IList;
                        var foundAssets = new List<object>();
                
                        foreach (var link in elements)
                        {
                            hasAssetLink = true;
                            foundAssets.Add(refreshLinkGeneric((AssetLink)link, linkType));
                        }
                
                        for (int i = 0; i < foundAssets.Count; i++)
                        {
                            elements[i] = foundAssets[i];
                            field.SetValue(obj, elements);
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