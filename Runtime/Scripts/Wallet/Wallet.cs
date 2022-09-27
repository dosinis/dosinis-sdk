using DosinisSDK.Core;

namespace DosinisSDK.Wallet
{
    public class Wallet : Module
    {
        public Currency SoftCurrency { get; private set; }
        public Currency HardCurrency { get; private set; }

        private WalletData data;
        private WalletConfig config;

        protected override void OnInit(IApp app)
        {
            config = GetConfigAs<WalletConfig>();
            
            var dataManager = app.GetModule<IDataManager>();
            
            SoftCurrency = config.softCurrency;
            HardCurrency = config.hardCurrency;
            
            if (dataManager.HasData<WalletData>() == false)
            {
                data = dataManager.RetrieveOrCreateData<WalletData>();
                
                data.softCurrency.amount = SoftCurrency.initAmount;
                
                if (HardCurrency)
                {
                    data.hardCurrency.amount = HardCurrency.initAmount;
                }
            }
            else
            {
                data = dataManager.RetrieveOrCreateData<WalletData>();
            }
            
            SoftCurrency.Set(data.softCurrency);
            
            if (HardCurrency)
            {
                HardCurrency.Set(data.hardCurrency);
            }
        }
    }
}