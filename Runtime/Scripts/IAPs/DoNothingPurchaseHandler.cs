using UnityEngine;

namespace DosinisSDK.IAPs
{
    [CreateAssetMenu(fileName = "DoNothingPurchaseHandler", menuName = "DosinisSDK/IAPs/DoNothingPurchaseHandler")]
    public class DoNothingPurchaseHandler : PurchaseHandler
    {
        // NOTE: This can be used to register IAP, but handling rewarding separately
        
        protected override void OnRewarded()
        {
        }

        public override string GetValueString()
        {
            return "";
        }
    }
}
