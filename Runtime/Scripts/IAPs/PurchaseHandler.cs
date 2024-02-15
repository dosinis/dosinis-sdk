using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace DosinisSDK.IAPs
{
    public abstract class PurchaseHandler : ScriptableObject
    {
        [SerializeField] private string productId;
        [SerializeField] private ProductType productType;
        [SerializeField] private PurchaseHandler[] extraHandlers;

        public string ProductId => productId;
        public ProductType ProductType => productType;
        public IReadOnlyCollection<PurchaseHandler> ExtraHandlers => extraHandlers;
        public virtual bool IsListed => true;

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

        public virtual string GetValueString()
        {
            return "";
        }
    }
}
