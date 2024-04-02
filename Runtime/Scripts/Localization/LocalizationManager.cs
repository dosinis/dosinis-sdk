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

        // key -> value
        private readonly Dictionary<string, string> localizationTable = new();
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
            
            PopulateDictionary(config.CachedLocalizationTsv);

            var result = await restManager.GetAsync(config.TsvUrl);
            
            if (result.result == Result.Success)
            {
                downloadedTsv = result.resultString;
                PopulateDictionary(downloadedTsv);
            }
            else
            {
                LogError("Failed to download localization data: " + result.error);
            }
        }
        
        /// <summary>
        /// tsv string must be formatted in such way:
        /// Key Lang1   Lang2   Lang3...
        /// #key    translation1    translation2    translation3...
        /// </summary>
        /// <param name="tsv"></param>
        private void PopulateDictionary(string tsv)
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

        public void SetLanguage(string language)
        {
            data.language = language;
            CurrentLanguage = data.language;
            
            if (string.IsNullOrEmpty(downloadedTsv) == false)
            {
                PopulateDictionary(downloadedTsv);
            }
            else
            {
                PopulateDictionary(config.CachedLocalizationTsv);
            }
            
            OnLanguageChanged?.Invoke();
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