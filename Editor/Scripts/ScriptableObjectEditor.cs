using DosinisSDK.Inspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DosinisSDK.Editor
{
    [CustomEditor(typeof(ScriptableObject), true), CanEditMultipleObjects]
    public class ScriptableObjectEditor : UnityEditor.Editor
    {
        private ScriptableObject targetObject;
        private MemberInfo[] methods;

        private void ButtonInspector()
        {
            var methodsWithAttribute = GetMethodsWithAttribute(typeof(ButtonAttribute));

            EditorGUILayout.Space(10);

            foreach (var memberInfo in methodsWithAttribute)
            {
                DrawButton(memberInfo as MethodInfo);
            }
        }

        private void DrawButton(MethodInfo methodInfo)
        {
            var buttonName = methodInfo.Name;

            if (GUILayout.Button(buttonName))
            {
                methodInfo.Invoke(targetObject, null);
            }
        }

        private MemberInfo[] GetMethods()
        {
            return targetObject.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Instance
                | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private IEnumerable<MemberInfo> GetMethodsWithAttribute(Type attributeType)
        {
            methods ??= GetMethods();

            return methods.Where(member => Attribute.IsDefined(member, attributeType));
        }

        // Editor

        protected virtual void OnEnable()
        {
            targetObject = target as ScriptableObject;

            if (targetObject)
            {
                methods = GetMethods();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!targetObject)
            {
                EditorGUILayout.HelpBox("ScriptableObject is not properly loaded, try selecting it again.", MessageType.Error);
                return;
            }

            ButtonInspector();
        }
    }
}