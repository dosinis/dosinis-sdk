using DosinisSDK.Core;
using System.Collections.Generic;

namespace DosinisSDK.Wallet
{
    [System.Serializable]
    public class WalletData : ModuleData
    {
        public Currency softCurrency;
        public Currency hardCurrency;
    }
}