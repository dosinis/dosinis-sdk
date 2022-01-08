using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Wallet
{
    [CreateAssetMenu(fileName = "WalletConfig", menuName = "DosinisSDK/Wallet/WalletConfig")]
    public class WalletConfig : ModuleConfig
    {
        public CurrencyConfig[] currencies;
    }

    [System.Serializable]
    public struct CurrencyConfig
    {
        public int initAmount;
        public Sprite icon;
        public string name;
    }
}

