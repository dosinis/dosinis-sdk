using DosinisSDK.Core;
using System;

namespace DosinisSDK.Ads
{
    public interface IAdsManager : IBehaviourModule
    {
        event Action OnBannerLoaded;
        event Action<string> OnInterstitialShown;
        event Action<string> OnRewardedShown;
        bool IsBannerDisplayed { get; set; }
        bool IsRewardAdReady();
        void ShowRewardedAd(string placement, Action<bool> callBack);
        void ShowInterstitial(string placement = "");
        void ShowBanner();
        void HideBanner();
    }
}


