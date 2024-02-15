using System;
using System.Collections.Generic;
using DosinisSDK.Core;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace DosinisSDK.IAPs
{
    public class IAPManager : Module, IIAPManager, IStoreListener
    {
        // Private

        private IStoreController storeController;
        private IExtensionProvider storeExtensionProvider;
        private readonly Dictionary<string, ProductData> productsRegistry = new Dictionary<string, ProductData>();
        private IAPConfig config;

        // Properties
        
        public bool Initialized => storeController != null && storeExtensionProvider != null;

        // Events

        public event Action<string> OnProductPurchased;

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

            foreach (var handler in config.PurchaseHandlers)
            {
                RegisterProduct(handler.ProductId, handler.ProductType, handler.HandlePurchase);
            }

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var product in productsRegistry)
            {
                builder.AddProduct(product.Key, product.Value.type);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        private void RegisterProduct(string productId, ProductType productType, Action purchaseCallback)
        {
            productsRegistry.Add(productId, new ProductData
            {
                type = productType,
                purchaseCallback = purchaseCallback
            });
        }

        public void PurchaseProduct(string productId)
        {
            if (Application.isEditor)
            {
                if (productsRegistry.ContainsKey(productId))
                {
                    productsRegistry[productId].purchaseCallback?.Invoke();
                    OnProductPurchased?.Invoke(productId);
                }

                return;
            }

            if (Initialized)
            {
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

        public void RestorePurchases()
        {
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
            // NOTE: if this won't work in Android, loop through all items in storeController and see if they have receipt, if they do - restore it
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

        // IStoreListener implementation

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Log("Initialized");
            storeController = controller;
            storeExtensionProvider = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Warn($"Initialize failed. InitializationFailureReason: {error}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Warn($"Initialize failed. InitializationFailureReason: {error} {message}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            var product = args.purchasedProduct;

            bool validPurchase = true;

            var validator = new CrossPlatformValidator(GooglePublicKeyHandler(),
                ApplePublicKeyHandler(), Application.identifier);

            try
            {
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

            if (validPurchase == false)
            {
                Warn("Couldn't validate purchase. Not granting any reward");
                return PurchaseProcessingResult.Complete;
            }

            string productId = product.definition.id;

            if (productsRegistry.ContainsKey(productId))
            {
                productsRegistry[productId].purchaseCallback?.Invoke();
            }

            Log($"ProcessPurchase: Complete. Product: {args.purchasedProduct.definition.id} - {product.transactionID}");

            OnProductPurchased?.Invoke(productId);

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Warn($"OnPurchaseFailed: Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureReason}");
        }

        private struct ProductData
        {
            public ProductType type;
            public Action purchaseCallback;
        }
    }
}