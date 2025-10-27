using System;
using DosinisSDK.Core;

namespace DosinisSDK.IAP
{
    public struct ProductData
    {
        public ProductType type;
        public Action<IModulesProvider> purchaseCallback;
        public Action restoreCallback;
    }
}
