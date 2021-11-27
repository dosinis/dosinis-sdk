using DosinisSDK.Core;
using System;

namespace DosinisSDK.Ads
{
    public class DummyAdManager : AdManager
    {
        public override void OnInit(IApp app)
        {
            
        }

        public override void ShowBanner(string placement = "")
        {
            Log($"Show banner ad {placement}");
        }

        public override void ShowInterstitial(string placement = "")
        {
            Log($"Show interstitial ad {placement}");
        }

        public override void ShowRewardedAd(string placement, Action<bool> callBack)
        {
            Log($"Show rewarded ad {placement}");

            App.Core.Timer.Delay(3f, () =>
            {
                callBack(true);
            });
        }

        protected override void InitializeRewardedAds()
        {
            Log("Init rewarded ads");
        }

        protected override void LoadBanner()
        {
            Log("Loading banner ads");
        }

        protected override void LoadInterstitialAds()
        {
            Log("Loading interstitial ads");
        }

        protected override void LoadRewardedAds()
        {
            Log("Loading rewarded ads");
        }
    }
}

