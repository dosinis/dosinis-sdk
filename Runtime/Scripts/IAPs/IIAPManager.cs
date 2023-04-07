using System;
using DosinisSDK.Core;
using UnityEngine.Purchasing;

namespace DosinisSDK.IAPs
{
    public interface IIAPManager : IModule
    {
        public bool Initialized { get; }
        event Action<string> OnProductPurchased;
        void PurchaseProduct(string productId);
        Product GetProductById(string productId);
        void RestorePurchases();
    }
}
