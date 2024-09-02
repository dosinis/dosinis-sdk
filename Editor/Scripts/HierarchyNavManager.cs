using UnityEditor;

namespace DosinisSDK.Editor
{
    public class HierarchyNavManager : UnityEditor.Editor
    {
        [MenuItem("Assets/Dosinis/MoveToRoot %g")]
        private static void MoveToRoot()
        {
            foreach (var s in Selection.transforms)
            {
                var root = s.root;
                s.SetParent(root);
                s.SetAsFirstSibling();
            }
        }
    }
}