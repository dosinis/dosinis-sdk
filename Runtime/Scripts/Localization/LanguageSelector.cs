using DosinisSDK.Core;
using DosinisSDK.Pool;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.Localization
{
    public class LanguageSelector : ManagedBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObjectPool languagePool;
        
        private ILocalizationManager localizationManager;
        
        protected override void OnInit(IApp app)
        {
            localizationManager = app.GetModule<ILocalizationManager>();

            foreach (var language in localizationManager.AvailableLanguages)
            {
                languagePool.Take<LanguageToggle>().Init(localizationManager, language);
            }
            
            closeButton.OnClick += Hide;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
