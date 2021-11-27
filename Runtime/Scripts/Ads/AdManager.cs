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

    public abstract class AdManager : BehaviourModule, IAdManager
    {
        [SerializeField] protected string rewardedId = "";
        [SerializeField] protected string interstitialId = "";
        [SerializeField] protected string bannerId = "";

        [SerializeField] protected bool showBanner;
        [SerializeField] protected BannerPosition bannerPosition;

        [SerializeField] protected bool useTestAds;

        public abstract void ShowRewardedAd(string placement, Action<bool> callBack);
        public abstract void ShowInterstitial(string placement = "");
        public abstract void ShowBanner(string placement = "");
        protected abstract void LoadRewardedAds();
        protected abstract void LoadInterstitialAds();
        protected abstract void LoadBanner();
        protected abstract void InitializeRewardedAds();
    }
}


