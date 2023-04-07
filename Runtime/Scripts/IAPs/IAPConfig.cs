using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.IAPs
{
    [CreateAssetMenu(menuName = "DosinisSDK/IAPs/IAPConfig", fileName = "IAPConfig")]
    public class IAPConfig : ModuleConfig
    {
        public PurchaseHandler[] purchaseHandlers;
    }
}
