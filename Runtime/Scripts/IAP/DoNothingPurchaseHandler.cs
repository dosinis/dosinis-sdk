using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.IAP
{
    [CreateAssetMenu(fileName = "DoNothingPurchaseHandler", menuName = "DosinisSDK/IAPs/DoNothingPurchaseHandler")]
    public class DoNothingPurchaseHandler : PurchaseHandler
    {
        // NOTE: This can be used to register IAP, but handling rewarding separately
        
        protected override void OnRewarded(IModulesProvider modulesProvider)
        {
        }

        public override string GetValueString()
        {
            return "";
        }

        public override string GetDescription()
        {
            return "";
        }
    }
}
