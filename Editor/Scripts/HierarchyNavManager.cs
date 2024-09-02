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
            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                Debug.LogWarning("Can't move to root in prefab context mode!");
                return;
            }
            
            Transform root = null;
            
            foreach (var s in Selection.transforms)
            {
                root = s.root;
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