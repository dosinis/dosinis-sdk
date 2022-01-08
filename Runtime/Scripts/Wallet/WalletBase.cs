using DosinisSDK.Core;

namespace DosinisSDK.Wallet
{
    public class WalletBase : Module
    {
        // NOTE: Wallet data currencies list is mirrored by config currencies.
        // So in order to add new currency simply add new currency in config
        // and use it like this in your custom wallet:
        // public Currency Coins => data.currencies[0];

        protected WalletData data;
        private WalletConfig config;

        protected override void OnInit(IApp app)
        {
            config = GetConfigAs<WalletConfig>();
            data = app.GetModule<IDataManager>().RetrieveOrCreateData<WalletData>();

            if (config.currencies.Length > data.currencies.Count)
            {
                for (int i = data.currencies.Count; i < config.currencies.Length; i++)
                {
                    data.currencies.Add(new Currency(config.currencies[i].name, config.currencies[i].initAmount));
                }
            }

            for (int i = 0; i < data.currencies.Count; i++)
            {
                data.currencies[i].Icon = config.currencies[i].icon;
            }
        }
    }
}