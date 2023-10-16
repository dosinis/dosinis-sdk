using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DosinisSDK.Editor
{
    [InitializeOnLoad]
    public static class ToolbarExtender
    {
        private static readonly int toolCount;
        private static GUIStyle commandStyle = null;

        public static readonly List<Action> leftToolbarGUI = new List<Action>();
        public static readonly List<Action> rightToolbarGUI = new List<Action>();

        static ToolbarExtender()
        {
            Type toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");

#if UNITY_2019_1_OR_NEWER
            string fieldName = "k_ToolCount";
#else
			string fieldName = "s_ShownToolIcons";
#endif

            FieldInfo toolIcons = toolbarType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

#if UNITY_2019_3_OR_NEWER
            toolCount = toolIcons != null ? ((int)toolIcons.GetValue(null)) : 8;
#elif UNITY_2019_1_OR_NEWER
			toolCount = toolIcons != null ? ((int) toolIcons.GetValue(null)) : 7;
#elif UNITY_2018_1_OR_NEWER
			toolCount = toolIcons != null ? ((Array) toolIcons.GetValue(null)).Length : 6;
#else
			toolCount = toolIcons != null ? ((Array) toolIcons.GetValue(null)).Length : 5;
#endif

            ToolbarCallback.onToolbarGUI = OnGUI;
            ToolbarCallback.onToolbarGUILeft = GUILeft;
            ToolbarCallback.onToolbarGUIRight = GUIRight;
        }

#if UNITY_2019_3_OR_NEWER
        private const float SPACE = 8;
#else
		private const float SPACE = 10;
#endif
        private const float LARGE_SPACE = 20;
        private const float BOTTOM_WIDTH = 32;
        private const float DROPDOWN_WIDTH = 80;
#if UNITY_2019_1_OR_NEWER
        private const float PLAY_PAUSE_STOP_WIDTH_WIDTH = 140;
#else
		private const float PLAY_PAUSE_STOP_WIDTH_WIDTH = 100;
#endif

        static void OnGUI()
        {
            if (commandStyle == null)
            {
                commandStyle = new GUIStyle("CommandLeft");
            }

            var screenWidth = EditorGUIUtility.currentViewWidth;
            
            float playButtonsPosition = Mathf.RoundToInt((screenWidth - PLAY_PAUSE_STOP_WIDTH_WIDTH) / 2);

            Rect leftRect = new Rect(0, 0, screenWidth, Screen.height);
            leftRect.xMin += SPACE;
            leftRect.xMin += BOTTOM_WIDTH * toolCount;
#if UNITY_2019_3_OR_NEWER
            leftRect.xMin += SPACE;
#else
			leftRect.xMin += LARGE_SPACE; // Spacing between tools and pivot
#endif
            leftRect.xMin += 64 * 2;
            leftRect.xMax = playButtonsPosition;

            Rect rightRect = new Rect(0, 0, screenWidth, Screen.height);
            rightRect.xMin = playButtonsPosition;
            rightRect.xMin += commandStyle.fixedWidth * 3;
            rightRect.xMax = screenWidth;
            rightRect.xMax -= SPACE; 
            rightRect.xMax -= DROPDOWN_WIDTH; 
            rightRect.xMax -= SPACE;
            rightRect.xMax -= DROPDOWN_WIDTH;
#if UNITY_2019_3_OR_NEWER
            rightRect.xMax -= SPACE;
#else
			rightRect.xMax -= LARGE_SPACE; // Spacing between layers and account
#endif
            rightRect.xMax -= DROPDOWN_WIDTH;
            rightRect.xMax -= SPACE;
            rightRect.xMax -= BOTTOM_WIDTH;
            rightRect.xMax -= SPACE;
            rightRect.xMax -= 78;
            
            leftRect.xMin += SPACE;
            leftRect.xMax -= SPACE;
            rightRect.xMin += SPACE;
            rightRect.xMax -= SPACE;

#if UNITY_2019_3_OR_NEWER
            leftRect.y = 4;
            leftRect.height = 22;
            rightRect.y = 4;
            rightRect.height = 22;
#else
			leftRect.y = 5;
			leftRect.height = 24;
			rightRect.y = 5;
			rightRect.height = 24;
#endif

            if (leftRect.width > 0)
            {
                GUILayout.BeginArea(leftRect);
                GUILayout.BeginHorizontal();
                foreach (var handler in leftToolbarGUI)
                {
                    handler();
                }

                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }

            if (rightRect.width > 0)
            {
                GUILayout.BeginArea(rightRect);
                GUILayout.BeginHorizontal();
                foreach (var handler in rightToolbarGUI)
                {
                    handler();
                }

                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }

        private static void GUILeft()
        {
            GUILayout.BeginHorizontal();
            
            foreach (var handler in leftToolbarGUI)
            {
                handler();
            }

            GUILayout.EndHorizontal();
        }

        private static void GUIRight()
        {
            GUILayout.BeginHorizontal();
            
            foreach (var handler in rightToolbarGUI)
            {
                handler();
            }

            GUILayout.EndHorizontal();
        }
        
        public static void RegisterRightEntry(Action right, int order)
        {
            if (order > rightToolbarGUI.Count)
            {
                rightToolbarGUI.Add(right);
                return;
            }
		    
            if (order < 0)
            {
                order = 0;
            }
		    
            rightToolbarGUI.Insert(order, right);
        }
    }
}