using DosinisSDK.Core;
using DosinisSDK.Utils;
using TMPro;
using UnityEngine;

namespace DosinisSDK.Localization
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : ManagedBehaviour
    {
        [SerializeField] private string key;
        [SerializeField] private TMP_Text text;

        public string Key => key;
        
        private ILocalizationManager localizationManager;
        
        protected override void OnInit(IApp app)
        {
            localizationManager = app.GetModule<ILocalizationManager>();
            localizationManager.OnLanguageChanged += OnLanguageChanged;
            
            OnLanguageChanged();
        }

        private void OnDestroy()
        {
            if (localizationManager != null)
            {
                localizationManager.OnLanguageChanged -= OnLanguageChanged;
            }
        }

        private void OnLanguageChanged()
        {
            text.text = localizationManager.GetLocalizedString(key);
        }

        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }

        public void SetKey(string key)
        {
            this.key = key;
            OnLanguageChanged();
        }
    }
}
