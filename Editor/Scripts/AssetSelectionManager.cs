using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DosinisSDK.Editor
{
    public class AssetSelectionManager : UnityEditor.Editor
    {
        private const int MAX_CACHE_SIZE = 64;
        private static readonly List<Object> selectedAssets = new List<Object>();
        private static int selectionIndex = -1;
        
        [MenuItem("Assets/Dosinis/NavigateBack %q")]
        private static void NavigateBack()
        {
            if (selectionIndex > 0)
            {
                selectionIndex--;
                Selection.activeObject = selectedAssets[selectionIndex];
            }
        }

        [MenuItem("Assets/Dosinis/NavigateForward %e")]
        private static void NavigateForward()
        {
            if (selectionIndex < selectedAssets.Count - 1)
            {
                selectionIndex++;
                Selection.activeObject = selectedAssets[selectionIndex];
            }
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            Selection.selectionChanged += UpdateSelectedAssets;
        }

        private static void UpdateSelectedAssets()
        {
            var selectedAsset = Selection.activeObject;

            RegisterAsset(selectedAsset);
        }
        
        private static void RegisterAsset(Object selectedAsset)
        {
            if (selectedAsset != null)
            {
                if (selectedAssets.Count >= MAX_CACHE_SIZE)
                {
                    selectedAssets.RemoveAt(0);
                }

                selectedAssets.Add(selectedAsset);

                selectionIndex = selectedAssets.IndexOf(selectedAsset);
            }
        }
    }
}