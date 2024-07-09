using System;
using System.Collections.Generic;
using DosinisSDK.Core;
using DosinisSDK.Rest;
using UnityEngine;

namespace DosinisSDK.Localization
{
    public class LocalizationManager : Module, ILocalizationManager
    {
        private LocalizationConfig config;
        private LocalizationData data;
        
        // key - translation
        private readonly Dictionary<string, string> localizationTable = new();
        // key - audio clip
        private readonly Dictionary<string, AudioClip> localizedAudio = new();
        private readonly List<string> availableLanguages = new();
        private string downloadedTsv;

        public event Action OnLanguageChanged;
        
        // Properties
        
        public IReadOnlyCollection<string> AvailableLanguages => availableLanguages;
        public string CurrentLanguage { get; private set; }

        protected override async void OnInit(IApp app)
        {
            config = GetConfigAs<LocalizationConfig>();
            data = app.DataManager.GetOrCreateData<LocalizationData>();
            var restManager = app.GetModule<IRestManager>();

            if (string.IsNullOrEmpty(data.language))
            {
                data.language = Application.systemLanguage.ToString();
            }
            
            CurrentLanguage = data.language;
            
            PopulateTextTable(config.CachedLocalizationTsv);

            var result = await restManager.GetAsync(config.TsvUrl);
            
            if (result.result == Result.Success)
            {
                downloadedTsv = result.resultString;
                PopulateTextTable(downloadedTsv);
            }
            else
            {
                LogError("Failed to download localization data: " + result.error);
            }

            CacheAudio();
        }
        
        /// <summary>
        /// tsv string must be formatted in such way:
        /// Key Lang1   Lang2   Lang3...
        /// #key    translation1    translation2    translation3...
        /// </summary>
        /// <param name="tsv"></param>
        private void PopulateTextTable(string tsv)
        {
            localizationTable.Clear();
            availableLanguages.Clear();
            
            var rows = tsv.Split('\n');
            var firstRow = rows[0].Split('\t');
            
            int targetColumn = 1;
            bool foundLanguage = false;
            
            for (int c = 1; c < firstRow.Length; c++)
            {
                var language = firstRow[c];
                
                availableLanguages.Add(language);

                if (CurrentLanguage == language)
                {
                    targetColumn = c;
                    foundLanguage = true;
                }
            }

            if (foundLanguage == false)
            {
                Warn($"Couldn't find {CurrentLanguage} in TSV. Using fallback language {firstRow[targetColumn]}");
                data.language = firstRow[targetColumn];
                CurrentLanguage = data.language;
            }

            for (int r = 1; r < rows.Length; r++)
            {
                var row = rows[r].Split('\t');
                localizationTable.Add(row[0], row[targetColumn]);
            }
        }

        private async void CacheAudio()
        {
            foreach (var localizedAudioClip in config.AudioClips)
            {
                var language = Enum.Parse<SystemLanguage>(CurrentLanguage);
                var clip = await localizedAudioClip.GetLocalizedAudioClip(language).GetAssetAsync<AudioClip>();
                localizedAudio[localizedAudioClip.Key] = clip;
            }
        }

        public void SetLanguage(string language)
        {
            data.language = language;
            CurrentLanguage = data.language;
            
            if (string.IsNullOrEmpty(downloadedTsv) == false)
            {
                PopulateTextTable(downloadedTsv);
            }
            else
            {
                PopulateTextTable(config.CachedLocalizationTsv);
            }

            CacheAudio();
            
            OnLanguageChanged?.Invoke();
        }

        public AudioClip GetLocalizedAudioClip(string key)
        {
            if (localizedAudio.TryGetValue(key, out var audioClip))
            {
                return audioClip;
            }

            return null;
        }
        
        public string GetLocalizedString(string key, string fallback = "")
        {
            if (localizationTable.TryGetValue(key, out var value))
            {
                return value;
            }

            return string.IsNullOrEmpty(fallback) == false ? fallback : key;
        }
        
        public string GetLocalizedStringWithArgs(string key, params string[] args)
        {
            var localizedString = GetLocalizedString(key);
            return string.Format(localizedString, args);
        }
    }
}