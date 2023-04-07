using UnityEngine;
using UnityEngine.Purchasing;

namespace DosinisSDK.IAPs
{
    public abstract class PurchaseHandler : ScriptableObject
    {
        public string productId;
        public ProductType productType;
        public PurchaseHandler[] extraHandlers;

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
