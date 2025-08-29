using System;
using DosinisSDK.Core;
using DosinisSDK.Localization;
using DosinisSDK.Utils;
using TMPro;
using UnityEngine;

namespace Localization
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : ManagedBehaviour
    {
        [SerializeField] private string key;
        [SerializeField] private TMP_Text text;

        public string Key => key;
        private string[] args = Array.Empty<string>();

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
            if (localizationManager == null) return;
            text.text = args.Length > 0
                ? localizationManager.GetLocalizedStringWithArgs(key, args)
                : localizationManager.GetLocalizedStringWithArgs(key);
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

        public void SetArgs(params string[] args)
        {
            this.args = args;
            OnLanguageChanged();
        }

        public void SetKeyWithArgs(string key, params string[] args)
        {
            this.key = key;
            this.args = args;
            OnLanguageChanged();
        }
    }
}