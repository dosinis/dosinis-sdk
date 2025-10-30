using System;
using System.Collections.Generic;
using DosinisSDK.Core;

namespace DosinisSDK.IAP
{
    public class DummyIAPManager : Module, IIAPManager
    {
        private readonly Dictionary<string, ProductData> productsRegistry = new();
        private ITimer timer;
        private IAPManagerConfig config;
        private IModulesProvider modulesProvider;
        
        public bool Initialized
        {
            get;
            private set;
        }
        
        public event Action OnStoreInitialized;
        public event Action<string> OnProductPurchased;
        
        protected override void OnInit(IApp app)
        {
            timer = app.Timer;
            config = GetConfigAs<IAPManagerConfig>();
            modulesProvider = app;
            
            timer.Delay(0.25f, () =>
            {
                OnStoreInitialized?.Invoke();
                Initialized = true;
            });
                  
            foreach (var handler in config.PurchaseHandlers)
            {
                RegisterProduct(handler.Id, handler.ProductType, handler.OnPurchased, handler.Restore);
            }
        }
        
        public void PurchaseProduct(string productId, Action<bool> onPurchased)
        {
            timer.Delay(0.25f, () =>
            {
                onPurchased?.Invoke(true);
                OnProductPurchased?.Invoke(productId);
                
                if (productsRegistry.TryGetValue(productId, out var productData))
                {
                    productData.purchaseCallback?.Invoke(modulesProvider);
                }
                else
                {
                    LogError($"Could not find product with id {productId} in the registry.");
                }
            });
        }

        public string GetProductPrice(string productId)
        {
            return "9.99$";
        }

        public string GetProductTitle(string productId)
        {
            return "";
        }

        public bool IsPurchased(string productId)
        {
            return false;
        }

        public void RestorePurchases()
        {
        }

        public bool IsSubscribed(string productId)
        {
            return true;
        }

        public void RegisterProduct(string productId, ProductType productType, Action<IModulesProvider> purchaseCallback, Action<IModulesProvider> restoreCallback)
        {
            productsRegistry[productId] = new ProductData
            {
                type = productType,
                purchaseCallback = purchaseCallback,
                restoreCallback = restoreCallback
            };
        }

        public PurchaseHandler GetPurchaseHandler(string productId)
        {
            foreach (var handler in config.PurchaseHandlers)
            {
                if (handler.Id == productId)
                {
                    return handler;
                }
            }

            return null;
        }
    }
}
