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

        private ILocalizationManager localizationManager;
        
        protected override void OnInit(IApp app)
        {
            localizationManager = app.GetModule<ILocalizationManager>();
            localizationManager.OnLanguageChanged += OnLanguageChanged;
            
            OnLanguageChanged();
        }

        private void OnDestroy()
        {
            localizationManager.OnLanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged()
        {
            text.text = localizationManager.GetLocalizedString(key);
        }

        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }
    }
}
