using DosinisSDK.Core;
using DosinisSDK.Utils;
using UnityEngine;
using UnityEngine.Purchasing;

namespace DosinisSDK.IAPs
{
    public abstract class PurchaseHandler : Reward
    {
        [SerializeField] private ProductType productType;
        
        public ProductType ProductType => productType;
        public virtual bool IsListed => App.Core.GetModule<IIAPManager>().IsPurchased(Id) == false;

        private void OnValidate()
        {
#if UNITY_ANDROID
            if (Id.Contains("-"))
            {
                Debug.LogWarning("PurchaseHandler: Product ID contains a dash. This is not allowed on Google Play. Use a _ instead.");
            }
#endif
        }
        
        public override string GetTitle()
        {
            var title = App.Core.GetModule<IIAPManager>().GetProductTitle(Id);
            
            if (string.IsNullOrEmpty(title) || title == $"Fake title for {Id}")
            {
                return base.GetTitle();
            }

            return title;
        }
    }
}
