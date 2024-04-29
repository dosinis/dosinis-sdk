using System;
using DosinisSDK.Core;
using UnityEngine.Purchasing;

namespace DosinisSDK.IAPs
{
    public interface IIAPManager : IModule
    {
        event Action OnStoreInitialized;
        public bool Initialized { get; }
        event Action<string> OnProductPurchased;
        void PurchaseProduct(string productId, Action<bool> onPurchased);
        Product GetProductById(string productId);
        string GetProductPrice(string productId);
        string GetProductTitle(string productId);
        bool IsPurchased(string productId);
        void RestorePurchases();
        bool IsSubscribed(string productId);
    }
}
