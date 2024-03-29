using System;
using DosinisSDK.Core;
using DosinisSDK.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace DosinisSDK.Ads
{
    public class DummyAdsManager : AdsManager
    {
        private bool rewardAdReady = false;

        private Canvas canvas;

        public override event Action OnBannerLoaded;
        public override event Action<string> OnInterstitialShown;
        public override event Action<string> OnRewardedShown;
        public override float LastTimeAnyAdFullyShown { get; protected set; }
        public override bool IsBannerDisplayed { get; protected set; }

        private ITimer timer;

        protected override void OnInit(IApp app)
        {
            this.timer = app.Timer;

            if (preLoadAds)
            {      
                LoadRewardedAds();
                LoadInterstitialAds();
            }
            
            if (showBannerOnInit)
            {
                LoadBanner();
                ShowBanner();
            }

            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
        }

        // Interstitial
        
        public override void ShowInterstitial(string placement = "")
        {
            OnInterstitialShown?.Invoke(placement);
            LastTimeAnyAdFullyShown = Time.time;
            Log($"Show interstitial ad {placement}");
        }

        public override void LoadInterstitialAds()
        {
            Log("Loading interstitial ads");
        }

        // Rewarded
        
        public override void ShowRewardedAd(string placement, Action<bool> callBack)
        {
            if (IsRewardAdReady() == false)
                return;

            Log($"Show rewarded ad {placement}");

            var ad = new GameObject("Rewarded Dummy");
            ad.transform.parent = canvas.transform;
            ad.AddComponent<Image>();

            var adRect = ad.GetComponent<RectTransform>();

            adRect.anchorMin = new Vector2(0, 0);
            adRect.anchorMax = new Vector2(1, 1);
            adRect.pivot = new Vector2(0.5f, 0.5f);

            adRect.SetLeft(0);
            adRect.SetRight(0);
            adRect.SetTop(0);
            adRect.SetBottom(0);

            OnRewardedShown?.Invoke(placement);
            
            Time.timeScale = 0;

            timer.Delay(3f, () =>
            {
                Time.timeScale = 1;
                rewardAdReady = false;

                Destroy(ad);
                LoadRewardedAds();

                callBack(true);
                LastTimeAnyAdFullyShown = Time.time;
            }, true);
        }
        
        public override void LoadRewardedAds()
        {
            Log("Loading rewarded ads");
            timer.Delay(3f, () =>
            {
                Log("Loaded rewarded ads");
                rewardAdReady = true;
            }, true);
        }
        
        public override bool IsRewardAdReady()
        {
            return rewardAdReady;
        }

        // Banner
        
        public override void LoadBanner()
        {
            Log("Loading banner ads");

            timer.Delay(3f, () =>
            {
                OnBannerLoaded?.Invoke();
            }, true);
        }
        
        public override void ShowBanner()
        {
            Log("Banner ad shown");
            IsBannerDisplayed = true;
        }

        public override void HideBanner()
        {
            Log("Banner ad hidden");
            IsBannerDisplayed = false;
        }
    }
}

