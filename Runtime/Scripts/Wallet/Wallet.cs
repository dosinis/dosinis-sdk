using DosinisSDK.Core;

namespace DosinisSDK.Wallet
{
    public class Wallet : Module
    {
        public Currency SoftCurrency => data.softCurrency;
        public Currency HardCurrency => data.hardCurrency;

        private WalletData data;
        private WalletConfig config;

        protected override void OnInit(IApp app)
        {
            config = GetConfigAs<WalletConfig>();
            data = app.GetModule<IDataManager>().RetrieveOrCreateData<WalletData>();

            data.softCurrency = new Currency(config.softCurrency.name, config.softCurrency.initAmount)
            {
                Icon = config.softCurrency.icon
            };

            data.hardCurrency = new Currency(config.hardCurrency.name, config.hardCurrency.initAmount)
            {
                Icon = config.hardCurrency.icon
            };
        }
    }
}