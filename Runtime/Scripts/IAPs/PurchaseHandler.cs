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
