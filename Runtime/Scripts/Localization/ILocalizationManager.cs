using System;
using System.Collections.Generic;
using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Localization
{
    public interface ILocalizationManager : IModule
    {
        IReadOnlyCollection<string> AvailableLanguages { get; }
        string CurrentLanguage { get; }
        void SetLanguage(string language);
        string GetLocalizedString(string key, string fallback = "");
        string GetLocalizedStringWithArgs(string key, params string[] args);
        AudioClip GetLocalizedAudioClip(string key);
        event Action OnLanguageChanged;
    }
}
