using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.IAP
{
    public abstract class PurchaseHandler : ScriptableObject
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private string id;
        [SerializeField] private string fallbackTitle;
        [SerializeField] private ProductType productType;

        public Sprite MainIcon => icon;
        public string Id => id;
        public ProductType ProductType => productType;

        public virtual bool IsListed => App.Core.GetModule<IIAPManager>().IsPurchased(id) == false;

        private void OnValidate()
        {
#if UNITY_ANDROID
            if (id.Contains("-"))
            {
                Debug.LogWarning("PurchaseHandler: Product ID contains a dash. This is not allowed on Google Play. Use a _ instead.");
            }
#endif
        }

        public abstract void OnPurchased(IModulesProvider modulesProvider);

        public virtual void Restore()
        {
            OnPurchased(App.Core);
        }
        
        public virtual string GetTitle()
        {
            var title = App.Core.GetModule<IIAPManager>().GetProductTitle(id);
            
            if (string.IsNullOrEmpty(title) || title == $"Fake title for {id}")
            {
                return fallbackTitle;
            }

            return title;
        }

        public abstract string GetValueString();
        public abstract string GetDescription();
    }
}
