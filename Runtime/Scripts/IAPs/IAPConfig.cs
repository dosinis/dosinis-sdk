using System.Collections.Generic;
using DosinisSDK.Core;
using DosinisSDK.Inspector;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.IAPs
{
    [CreateAssetMenu(menuName = "DosinisSDK/IAPs/IAPConfig", fileName = "IAPConfig")]
    public class IAPConfig : ModuleConfig
    {
        [SerializeField] private PurchaseHandler[] purchaseHandlers;

        public IReadOnlyCollection<PurchaseHandler> PurchaseHandlers => purchaseHandlers;

#if UNITY_EDITOR
        [Button]
        private void SetupHandlers()
        {
            EditorUtils.GetAssetsOfType(ref purchaseHandlers, this);
        }
#endif
    }
}