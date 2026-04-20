using UnityEngine;
using UnityEngine.Purchasing;
using DosinisSDK.IAP;

namespace DosinisSDK.UnityIAP
{
    public class UnityIAPConfig : IAPManagerConfig
    {
        [SerializeField] private FakeStoreUIMode fakeStoreUIMode = FakeStoreUIMode.StandardUser;
        
        public FakeStoreUIMode FakeStoreUIMode => fakeStoreUIMode;
    }
}
