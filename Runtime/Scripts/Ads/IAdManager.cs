using DosinisSDK.Core;
using System;

namespace DosinisSDK.Ads
{
    public interface IAdManager : IBehaviourModule
    {
        event Action OnBannerLoaded;
        bool IsRewardAdReady();
        void ShowRewardedAd(string placement, Action<bool> callBack);
        void ShowInterstitial(string placement = "");
        void ShowBanner(string placement = "");
    }
}


