using DosinisSDK.Core;
using System;

namespace DosinisSDK.Ads
{
    public interface IAdsManager : IModule
    {
        event Action OnBannerLoaded;
        event Action<string> OnInterstitialShown;
        event Action<string> OnRewardedShown;
        float LastTimeAnyAdFullyShown { get; }
        bool IsBannerDisplayed { get; }
        bool IsRewardAdReady();
        void ShowRewardedAd(string placement, Action<bool> callBack);
        void LoadRewardedAds();
        void ShowInterstitial(string placement = "");
        void LoadInterstitialAds();
        void ShowBanner();
        void HideBanner();
        void LoadBanner();
    }
}


