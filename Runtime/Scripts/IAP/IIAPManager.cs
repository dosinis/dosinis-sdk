using System;
using DosinisSDK.Core;

namespace DosinisSDK.IAP
{
    public interface IIAPManager : IModule
    {
        event Action OnStoreInitialized;
        public bool Initialized { get; }
        event Action<string> OnProductPurchased;
        void PurchaseProduct(string productId, Action<bool> onPurchased);
        string GetProductPrice(string productId);
        string GetProductTitle(string productId);
        bool IsPurchased(string productId);
        void RestorePurchases();
        bool IsSubscribed(string productId);
        void RegisterProduct(string productId, ProductType productType, 
            Action<IModulesProvider> purchaseCallback, Action restoreCallback);
    }
}
