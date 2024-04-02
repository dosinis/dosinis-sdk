using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DosinisSDK.Localization
{
    public class LanguageToggle : MonoBehaviour
    {
        [SerializeField] private TMP_Text languageTitle;
        [SerializeField] private Toggle toggle;
        
        private ILocalizationManager localizationManager;
        private string language;
        private bool initialized;
        
        private static readonly List<LanguageToggle> toggles = new();
        
        private void OnDestroy()
        {
            toggles.Remove(this);
            localizationManager.OnLanguageChanged -= OnLanguageChanged;
        }

        private void OnToggleValueChanged(bool value)
        {
            localizationManager.SetLanguage(language);
        }

        public void Init(ILocalizationManager localizationManager, string language)
        {
            if (initialized)
            {
                Debug.LogWarning("Language Toggle already initialized");
                return;
            }
            
            toggles.Add(this);
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
            
            this.localizationManager = localizationManager;
            this.language = language;
            languageTitle.text = language;

            OnLanguageChanged();
            
            localizationManager.OnLanguageChanged += OnLanguageChanged;
        }

        private void OnLanguageChanged()
        {
            foreach (var t in toggles)
            {
                t.toggle.SetIsOnWithoutNotify(t.language == localizationManager.CurrentLanguage);
            }
        }
    }
}
