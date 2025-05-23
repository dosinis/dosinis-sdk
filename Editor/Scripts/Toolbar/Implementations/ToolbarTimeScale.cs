using UnityEditor;
using UnityEngine;

namespace DosinisSDK.Editor
{
	[InitializeOnLoad]
	public class ToolbarTimeScale
	{
		private static GUIStyle textStyle;
		private const int TIME_SCALE = 5;
		
		static ToolbarTimeScale()
		{
			ToolbarExtender.leftToolbarGUI.Add(OnToolbarGUI);
			EditorApplication.playModeStateChanged += ModeStateChanged;
		}

		private static void ModeStateChanged(PlayModeStateChange state)
		{
			if(state == PlayModeStateChange.EnteredEditMode)
				Time.timeScale = 1;
		}

		private static void OnToolbarGUI()
		{
			if (textStyle == null)
			{
				textStyle = new  GUIStyle(EditorStyles.label)
				{
					alignment = TextAnchor.MiddleCenter,
					normal =
					{
						textColor = new Color(0.76f, 0.76f, 0.76f)
					},
					hover = 
					{
						textColor = Color.white
					}
				};
			}
			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Reset", EditorStyles.toolbarButton))
			{
				Time.timeScale = 1;
			}

			Time.timeScale = GUILayout.HorizontalSlider(Time.timeScale, 0, TIME_SCALE, 
				EditorStyles.toolbarButton, GUIStyle.none, GUILayout.Width(200));
			
			var rect = GUILayoutUtility.GetLastRect();

			var sliderRect = rect;
			var fillAmount = Time.timeScale / TIME_SCALE;

			sliderRect.width *= fillAmount;
			
			EditorGUI.DrawRect(sliderRect, new Color(0.4f, 0.4f, 0.4f));

			var time = $"{Time.timeScale:0.00}".Replace(",", ".");
			GUI.Label(rect, $"Time Scale: {time}", textStyle);

			var defaultRect = rect;

			defaultRect.width = 2;
			defaultRect.x += (rect.width * 1f / TIME_SCALE) - 1;
			
			EditorGUI.DrawRect(defaultRect, new Color(0.18f, 0.18f, 0.18f));
		}
	}
}