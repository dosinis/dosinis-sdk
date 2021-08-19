using DosinisSDK.Core;
using System;

namespace DosinisSDK.Ads
{
    public interface IAdManager : IBehaviourModule
    {
        void ShowRewardedAd(Action<bool> callBack, string placement = "");
        void ShowInterstitial(string placement = "");
        void ShowBanner(string placement = "");
    }
}


