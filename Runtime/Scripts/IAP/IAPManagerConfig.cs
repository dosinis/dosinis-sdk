using System.Collections.Generic;
using DosinisSDK.Core;
using DosinisSDK.Inspector;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.IAP
{
    [CreateAssetMenu(menuName = "DosinisSDK/IAPs/IAPManagerConfig", fileName = "IAPManagerConfig")]
    public class IAPManagerConfig : ModuleConfig
    {
        [SerializeField] private PurchaseHandler[] purchaseHandlers;
        
        public IReadOnlyList<PurchaseHandler> PurchaseHandlers => purchaseHandlers;

#if UNITY_EDITOR
        [Button]
        private void SetupHandlers()
        {
            EditorUtils.GetAssetsOfType(ref purchaseHandlers, this);
        }
#endif
    }
}