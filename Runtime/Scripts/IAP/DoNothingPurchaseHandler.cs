using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.IAP
{
    [CreateAssetMenu(fileName = "DoNothingPurchaseHandler", menuName = "DosinisSDK/IAPs/DoNothingPurchaseHandler")]
    public class DoNothingPurchaseHandler : PurchaseHandler
    {
        public override void OnPurchased(IModulesProvider modulesProvider)
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
