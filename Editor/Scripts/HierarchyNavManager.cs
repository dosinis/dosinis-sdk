using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DosinisSDK.Editor
{
    public class HierarchyNavManager : UnityEditor.Editor
    {
        [MenuItem("Assets/Dosinis/MoveToRoot %g")]
        private static void MoveToRoot()
        {
            Transform root = null;
            
            foreach (var s in Selection.transforms)
            {
                var prefabStage = PrefabStageUtility.GetPrefabStage(s.gameObject);

                root = prefabStage != null ? prefabStage.prefabContentsRoot.transform : s.root;
              
                s.SetParent(root);
                s.SetAsFirstSibling();
            }

            if (root)
            {
                EditorUtility.SetDirty(root);
            }
        }
    }
}