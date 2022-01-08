using DosinisSDK.Core;
using System.Collections.Generic;

namespace DosinisSDK.Wallet
{
    [System.Serializable]
    public class WalletData : ModuleData
    {
        public List<Currency> currencies = new List<Currency>();
    }
}