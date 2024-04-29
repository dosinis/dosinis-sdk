using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DosinisSDK.Core;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

namespace DosinisSDK.IAPs
{
    public class IAPManager : Module, IIAPManager, IDetailedStoreListener, IAsyncModule
    {
        // Private

        private IStoreController storeController;
        private IExtensionProvider storeExtensionProvider;
        private readonly Dictionary<string, ProductData> productsRegistry = new();
        private IAPConfig config;
        private bool moduleReady;

        // Properties

        public event Action OnStoreInitialized;
        public bool Initialized => storeController != null && storeExtensionProvider != null;

        // Events

        public event Action<string> OnProductPurchased;
        private Action<bool> purchaseCallback;
        
        // Static
        
        /// <summary>
        /// Get GooglePlay store public key from generated script - GooglePlayTangle.Data()
        /// </summary>
        public static Func<byte[]> GooglePublicKeyHandler { get; set; }

        /// <summary>
        /// Get Apple store public key from generated script - AppleTangle.Data()
        /// </summary>
        public static Func<byte[]> ApplePublicKeyHandler { get; set; }
        
        // IAPManager

        protected override void OnInit(IApp app)
        {
            config = GetConfigAs<IAPConfig>();

            if (Application.isEditor)
            {
                StandardPurchasingModule.Instance().useFakeStoreUIMode = config.FakeStoreUIMode;
            }
            
            if (GooglePublicKeyHandler == null || ApplePublicKeyHandler == null)
            {
                Warn("Public Key Handlers are not set. Please set it before initializing IAPManager");
            }
            
            foreach (var handler in config.PurchaseHandlers)
            {
                RegisterProduct(handler.Id, handler.ProductType, handler.GrantReward);
            }

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var product in productsRegistry)
            {
                builder.AddProduct(product.Key, product.Value.type);
            }
            
            UnityPurchasing.Initialize(this, builder);
        }

        public async Task InitAsync(IApp app)
        {
            while (moduleReady == false)
            {
                await Task.Yield();
            }
        }
        
        private void RegisterProduct(string productId, ProductType productType, Action purchaseCallback)
        {
            productsRegistry.Add(productId, new ProductData
            {
                type = productType,
                purchaseCallback = purchaseCallback
            });
        }

        public void PurchaseProduct(string productId, Action<bool> onPurchased = null)
        {
            if (Initialized)
            {
                purchaseCallback = onPurchased;
                
                var product = storeController.products.WithID(productId);

                if (product != null && product.availableToPurchase)
                {
                    Log($"Purchasing product: {product.definition.id}");
                    storeController.InitiatePurchase(product);
                }
                else
                {
                    Warn("PurchaseProduct: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                Warn("PurchaseProduct FAIL. Not initialized.");
            }
        }

        public Product GetProductById(string productId)
        {
            if (Initialized == false)
            {
                Warn("Store is not ready!");
                return default;
            }

            foreach (var product in storeController.products.all)
            {
                if (product.definition.id == productId)
                    return product;
            }

            Warn("Couldn't find product");
            return default;
        }

        public string GetProductPrice(string productId)
        {
            if (Initialized == false)
            {
                Warn("Store is not ready!");
                return "";
            }

            var product = GetProductById(productId);

            if (product == null)
            {
                Warn("Couldn't find product");
                return "";
            }

            return product.metadata.localizedPriceString;
        }

        public string GetProductTitle(string productId)
        {
            if (Initialized == false)
            {
                Warn("Store is not ready!");
                return "";
            }

            var product = GetProductById(productId);

            if (product == null)
            {
                Warn("Couldn't find product");
                return "";
            }

            var title = product.metadata.localizedTitle;
            
#if UNITY_ANDROID
            
            // NOTE: Remove "(app_name)" from title that is added by Google Play
            int lastIndex = title.LastIndexOf("(", StringComparison.Ordinal);
            
            if (lastIndex > 2)
            {
                title = title.Substring(0, lastIndex - 1).Trim();
            }
#endif

            return title;
        }

        public bool IsPurchased(string productId)
        {
            if (Initialized == false)
            {
                Warn("Store is not ready!");
                return false;
            }

            var product = GetProductById(productId);

            if (product == null)
            {
                Warn("Couldn't find product");
                return false;
            }
            
            return product.definition.type != ProductType.Consumable && product.hasReceipt;
        }

        public void RestorePurchases()
        {
            Log("Attempting to restore purchases");
            
            // NOTE: Restores all Non-consumable and Subscription products.
            // Upon restore ProcessPurchase will be called for each restored purchase.
#if UNITY_IOS
            storeExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((success, error) =>
            {
                if (success)
                {
                    Log("Restore purchases succeeded.");
                }
                else
                {
                    Warn($"Restore purchases failed. {error}");
                }
            });
#endif

#if UNITY_ANDROID
            storeExtensionProvider.GetExtension<IGooglePlayStoreExtensions>().RestoreTransactions((success, error) =>
            {
                if (success)
                {
                    Log("Restore purchases succeeded.");
                }
                else
                {
                    Warn($"Restore purchases failed. {error}");
                }
            });
#endif
        }

        public bool IsSubscribed(string productId)
        {
            if (Initialized == false)
            {
                Warn("Store is not ready!");
                return false;
            }
            
            var product = GetProductById(productId);

            if (product == null)
            {
                Warn("Couldn't find product");
                return false;
            }

            if (product.definition.type != ProductType.Subscription)
            {
                Warn($"Product {productId} is not a subscription.");
                return false;
            }
            
            if (product.hasReceipt)
            {
                return false;
            }
            
            try
            {
                // NOTE: The intro_json parameter is optional and is only used for the App Store to get introductory information.
                var subscriptionManager = new SubscriptionManager(product, null);

                var info = subscriptionManager.getSubscriptionInfo();

                return info.isSubscribed() == Result.True;
            }
            catch (Exception ex)
            {
                LogError($"IsSubscribed: {ex.Message}");
            }
            
            return false;
        }

        // IStoreListener implementation

        PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs args)
        {
            var product = args.purchasedProduct;

            bool validPurchase = true;
            
            if (Application.isEditor == false)
            {
                try
                {
                    var validator = new CrossPlatformValidator(GooglePublicKeyHandler(), ApplePublicKeyHandler(), Application.identifier);
                    
                    var result = validator.Validate(product.receipt);

                    Log("Receipt is valid. Contents:");

                    foreach (var productReceipt in result)
                    {
                        Log(productReceipt.productID);
                        Log(productReceipt.purchaseDate.ToString());
                        Log(productReceipt.transactionID);
                    }
                }
                catch (IAPSecurityException)
                {
                    validPurchase = false;
                }
            }
            
            if (validPurchase == false)
            {
                Warn("Couldn't validate purchase. Not granting any reward");
                purchaseCallback?.Invoke(false);
                return PurchaseProcessingResult.Complete;
            }

            string productId = product.definition.id;

            try
            {
                if (productsRegistry.TryGetValue(productId, out var data))
                {
                    data.purchaseCallback?.Invoke();
                }
            }
            catch (Exception ex)
            {
                LogError($"ProcessPurchase: Handler caused exception. {ex.Message}");
            }

            Log($"ProcessPurchase: Complete. Product: {args.purchasedProduct.definition.id} - {product.transactionID}");

            try
            {
                OnProductPurchased?.Invoke(productId);
            }
            catch (Exception ex)
            {
                LogError($"ProcessPurchase: OnProductPurchased event caused exception. {ex.Message}");
            }

            try
            {
                purchaseCallback?.Invoke(true);
            }
            catch (Exception ex)
            {
                LogError($"ProcessPurchase: purchaseCallback caused exception. {ex.Message}");
            }
           
            return PurchaseProcessingResult.Complete;
        }
        
        void IDetailedStoreListener.OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Warn($"OnPurchaseFailed: Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureDescription.message}");
            purchaseCallback?.Invoke(false);
        }
        
        void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Warn($"OnPurchaseFailed: Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureReason}");
            purchaseCallback?.Invoke(false);
        }
        
        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Log("Initialized");
            storeController = controller;
            storeExtensionProvider = extensions;
            moduleReady = true;
            
            OnStoreInitialized?.Invoke();
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        {
            Warn($"Initialize failed. InitializationFailureReason: {error}");
            moduleReady = true;
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Warn($"Initialize failed. InitializationFailureReason: {error} {message}");
            moduleReady = true;
        }
        
        private struct ProductData
        {
            public ProductType type;
            public Action purchaseCallback;
        }
    }
}