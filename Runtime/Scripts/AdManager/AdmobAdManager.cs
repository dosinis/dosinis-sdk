using DosinisSDK.Core;
using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Ads 
{
    public class AdmobAdManager : AdManager
    {
        private const int ADS_SIZE = 3;

        private RewardedAd[] rewardedAds = new RewardedAd[ADS_SIZE];

        public override void Init(IApp app)
        {
            throw new System.NotImplementedException();
        }

        public override void InitializeRewardedAds()
        {
            throw new System.NotImplementedException();
        }

        public override void LoadBanner()
        {
            throw new System.NotImplementedException();
        }

        public override void LoadInterstitialAds()
        {
            throw new System.NotImplementedException();
        }

        public override void LoadRewardedAds()
        {
            throw new System.NotImplementedException();
        }

        public override void SetBannerShown(bool value, string placement)
        {
            throw new System.NotImplementedException();
        }

        public override void ShowInterstitial(string placement)
        {
            throw new System.NotImplementedException();
        }

    }
}

