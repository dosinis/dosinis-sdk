using System.IO;
using DosinisSDK.Editor;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor.Saves
{
	[InitializeOnLoad]
    public class ToolbarSaves
    {
	    static ToolbarSaves()
	    {
		    ToolbarExtender.RegisterRightEntry(OnToolbarGUI, 0);
	    }

	    private static void OnToolbarGUI()
	    {
		    var cachedGUI = GUI.color;
		    
		    var configPath = Path.Combine("Assets", "Saves");
		    if (!AssetDatabase.IsValidFolder(configPath))
			    GUI.color = new Color(0.53f, 0.53f, 0.53f);
			    
		    var content = new GUIContent(EditorGUIUtility.IconContent("d_TreeEditor.Trash"));
		    var guiContent = new GUIContent( content.image);
		    if (GUILayout.Button(guiContent, EditorStyles.toolbarButton,GUILayout.Width(30.0f)))
		    {
			    if (Event.current.button == 0 && Event.current.type == EventType.Used)
			    {
				    if (AssetDatabase.IsValidFolder(configPath))
				    {
					    var result = true;
					    
					    if (Directory.GetFiles(Application.dataPath + "/Saves", "*.json").Length > 0)
					    {
						    result = EditorUtility.DisplayDialog("Delete save data",
							    "Are you sure you want to delete all save data?\n\nYou cannot undo the delete assets action", "Delete", "Cancel");
					    }
					    
					    if (result)
					    {
						    AssetDatabase.DeleteAsset(configPath);
						    PlayerPrefs.DeleteKey("data_wipe");
						    AssetDatabase.Refresh();
					    }
				    }   
			    }

			    if (Event.current.button == 1 && Event.current.type == EventType.Used)
			    {
				    string folderPath = Application.dataPath + "/Saves";

				    if (!Directory.Exists(folderPath))
					    return;
				    
				    string[] filePaths = Directory.GetFiles(folderPath, "*.json");

				    var menu = new GenericMenu();
				    foreach (var path in filePaths)
				    {
					    string fileName = Path.GetFileNameWithoutExtension(path);
					    
					    menu.AddItem(new GUIContent($"{fileName}"), false, () =>
					    {
						    string assetPath = "Assets" + path.Substring(Application.dataPath.Length);
						    AssetDatabase.DeleteAsset(assetPath);
						    AssetDatabase.Refresh();

						    string[] paths = Directory.GetFiles(folderPath, "*.json");
						    if (paths.Length == 0)
						    {
							    AssetDatabase.DeleteAsset(configPath);
							    AssetDatabase.Refresh();
						    }
					    });
				    }
				    
				    menu.ShowAsContext();
			    }
		    }

		    GUI.color = cachedGUI;
	    }
    }
}