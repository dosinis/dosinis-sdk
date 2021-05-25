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

        [SerializeField] private string rewardedId = "";
        [SerializeField] private string interstitialId = "";
        [SerializeField] private string bannerId = "";

        [SerializeField] private bool showBanner;
        [SerializeField] private BannerPosition bannerPosition;

        [SerializeField] private bool useTestAds;

        private event Action<bool> OnRewardedAdFinished = b => { };

        public virtual void ShowRewardedAd(string placement, Action<bool> callBack)
        {
            void CallBack(bool success)
            {
                callBack(success);
                OnRewardedAdFinished -= CallBack;
            }

            OnRewardedAdFinished += CallBack;
        }

        public abstract void ShowInterstitial(string placement);
        public abstract void SetBannerShown(bool value, string placement);
        public abstract void LoadRewardedAds();
        public abstract void LoadInterstitialAds();
        public abstract void LoadBanner();
        public abstract void InitializeRewardedAds();

    }
}


