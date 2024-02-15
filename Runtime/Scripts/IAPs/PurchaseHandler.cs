using System.Collections.Generic;
using DosinisSDK.Core;
using UnityEngine;
using UnityEngine.Purchasing;

namespace DosinisSDK.IAPs
{
    public abstract class PurchaseHandler : ScriptableObject
    {
        [SerializeField] private string productId;
        [SerializeField] private ProductType productType;
        [SerializeField] private PurchaseHandler[] extraHandlers;
        [SerializeField] private string fallbackTitle;
        
        public string ProductId => productId;
        public ProductType ProductType => productType;
        public IReadOnlyCollection<PurchaseHandler> ExtraHandlers => extraHandlers;
        public virtual bool IsListed => App.Core.GetModule<IIAPManager>().IsPurchased(productId) == false;

        internal void HandlePurchase()
        {
            OnPurchased();
            
            if (extraHandlers == null)
                return;
            
            foreach (var handler in extraHandlers)
            {
                if (handler == this)
                {
                    Debug.LogError("PurchaseHandler: Extra handler is referencing to itself");
                    continue;    
                }

                handler.HandlePurchase();
            }
        }

        protected abstract void OnPurchased();
        public abstract string GetValueString();
        
        public virtual string GetTitle()
        {
            var title = App.Core.GetModule<IIAPManager>().GetProductTitle(productId);
            
            if (string.IsNullOrEmpty(title) || title == $"Fake title for {productId}")
            {
                return fallbackTitle;
            }

            return title;
        }
    }
}
