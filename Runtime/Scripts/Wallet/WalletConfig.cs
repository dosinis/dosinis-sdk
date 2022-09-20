using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Wallet
{
    [CreateAssetMenu(fileName = "WalletConfig", menuName = "DosinisSDK/Configs/Wallet/WalletConfig")]
    public class WalletConfig : ModuleConfig
    {
        public Currency softCurrency;
        public Currency hardCurrency;
    }
}

