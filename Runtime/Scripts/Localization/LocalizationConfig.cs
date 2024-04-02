using DosinisSDK.Core;
using DosinisSDK.Inspector;
using DosinisSDK.Rest;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DosinisSDK.Localization
{
    [CreateAssetMenu(menuName = "DosinisSDK/Localization/LocalizationConfig", fileName = "LocalizationConfig")]
    public class LocalizationConfig : ModuleConfig
    {
        [SerializeField] private string tsvUrl;
        [SerializeField, TextArea(5, 30)] private string localizationTsv;
        
        public string TsvUrl => tsvUrl;
        public string CachedLocalizationTsv => localizationTsv;

#if UNITY_EDITOR
        [Button]
        private async void DownloadLocalization()
        {
            var restManager = new RestManager();

            var response = await restManager.GetAsync(tsvUrl);

            if (response.result == Result.Success)
            {
                localizationTsv = response.resultString;
            }
            else
            {
                Debug.LogError($"Failed to download localization data: {response.error}");
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}