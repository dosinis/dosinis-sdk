using DosinisSDK.Inspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DosinisSDK.Editor
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class MonoBehaviourEditor : UnityEditor.Editor
    {
        private MonoBehaviour targetObject;
        private MemberInfo[] methods;

        private void ButtonInspector()
        {
            IEnumerable<MemberInfo> methods = GetMethodsWithAttribute(typeof(ButtonAttribute));

            EditorGUILayout.Space(10);

            foreach (var memberInfo in methods)
            {
                DrawButton(memberInfo as MethodInfo);
            }
        }

        private void DrawButton(MethodInfo methodInfo)
        {
            var buttonName = methodInfo.Name;

            if (GUILayout.Button(buttonName, GUILayout.Height(16)))
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
            methods = GetMethods();

            return methods.Where(member => Attribute.IsDefined(member, attributeType));
        }

        // Editor

        private void OnEnable()
        {
            targetObject = target as MonoBehaviour;

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
                EditorGUILayout.HelpBox("MonoBehaviour is not properly loaded, try selecting it again.", MessageType.Error);
                return;
            }

            ButtonInspector();
        }
    }
}