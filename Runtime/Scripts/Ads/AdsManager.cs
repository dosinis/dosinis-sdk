using DosinisSDK.Core;
using System;
using UnityEngine;

namespace DosinisSDK.Ads
{
    public enum BannerPosition
    {
        Top,
        Bottom
    }

    public abstract class AdsManager : BehaviourModule, IAdsManager
    {
        [SerializeField] protected string rewardedId = "";
        [SerializeField] protected string interstitialId = "";
        [SerializeField] protected string bannerId = "";

        [SerializeField] protected bool showBanner;
        [SerializeField] protected BannerPosition bannerPosition;

        [SerializeField] protected bool useTestAds;

        public abstract event Action OnBannerLoaded;
        public abstract event Action<string> OnInterstitialShown;
        public abstract event Action<string> OnRewardedShown;
        
        public bool IsBannerDisplayed { get; set; }

        public abstract bool IsRewardAdReady();
        protected abstract void LoadRewardedAds();
        public abstract void ShowRewardedAd(string placement, Action<bool> callBack);
        protected abstract void LoadInterstitialAds();
        public abstract void ShowInterstitial(string placement = "");
        public abstract void ShowBanner();
        public abstract void HideBanner();
        protected abstract void LoadBanner();
    }
}

