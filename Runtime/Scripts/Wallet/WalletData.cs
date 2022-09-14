using DosinisSDK.Core;

namespace DosinisSDK.Wallet
{
    [System.Serializable]
    public class WalletData : ModuleData
    {
        public CurrencyRef softCurrency = new CurrencyRef();
        public CurrencyRef hardCurrency = new CurrencyRef();
    }
    
    [System.Serializable]
    public class CurrencyRef
    {
        public int amount;
    }
}