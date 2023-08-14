using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace DosinisSDK.Editor
{
    public class AssetSelectionManager : UnityEditor.Editor
    {
        private const int MAX_CACHE_SIZE = 64;
        private static readonly List<Object> selectedAssets = new List<Object>();
        private static int selectionIndex = -1;


        [MenuItem("Assets/SelectPrevious %q")]
        private static void SelectPrevious()
        {
            if (selectionIndex > 0)
            {
                selectionIndex--;
                Selection.activeObject = selectedAssets[selectionIndex];
            }
        }

        [MenuItem("Assets/SelectNext %e")]
        private static void SelectNext()
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

            if (selectedAsset != null && selectedAsset is GameObject == false)
            {
                if (!selectedAssets.Contains(selectedAsset))
                {
                    if (selectedAssets.Count >= MAX_CACHE_SIZE)
                    {
                        selectedAssets.RemoveAt(0);
                    }

                    selectedAssets.Add(selectedAsset);
                }

                selectionIndex = selectedAssets.IndexOf(selectedAsset);
            }
        }
    }
}