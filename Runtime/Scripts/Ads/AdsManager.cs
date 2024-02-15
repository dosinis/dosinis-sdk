using DosinisSDK.Core;
using System;
using DosinisSDK.Inspector;
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
        [SerializeField] protected bool useTestAds;
        
        [SerializeField, ShowIf("useTestAds", false)] protected string rewardedId = "";
        [SerializeField, ShowIf("useTestAds", false)] protected string interstitialId = "";
        [SerializeField, ShowIf("useTestAds", false)] protected string bannerId = "";

        [SerializeField] protected bool showBanner;
        [SerializeField] protected BannerPosition bannerPosition;

        public abstract event Action OnBannerLoaded;
        public abstract event Action<string> OnInterstitialShown;
        public abstract event Action<string> OnRewardedShown;
        public abstract float LastTimeAnyAdFullyShown { get; protected set; }
        public abstract bool IsBannerDisplayed { get; protected set; }

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


