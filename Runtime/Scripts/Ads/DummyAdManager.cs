using DosinisSDK.Core;
using DosinisSDK.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace DosinisSDK.Ads
{
    public class DummyAdManager : AdManager
    {
        private bool rewardAdReady = false;

        private Canvas canvas;

        public override event Action OnBannerLoaded;

        protected override void OnInit(IApp app)
        {
            LoadRewardedAds();
            LoadInterstitialAds();

            if (showBanner)
            {
                ShowBanner();
                LoadBanner();
            }

            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
        }

        public override void ShowBanner(string placement = "")
        {
            Log($"Show banner ad {placement}");
        }

        public override void ShowInterstitial(string placement = "")
        {
            Log($"Show interstitial ad {placement}");
        }

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

            Time.timeScale = 0;

            App.Core.Timer.Delay(3f, () =>
            {
                Time.timeScale = 1;
                rewardAdReady = false;

                Destroy(ad);
                LoadRewardedAds();

                callBack(true);
            });
        }

        protected override void LoadBanner()
        {
            Log("Loading banner ads");

            App.Core.Timer.Delay(3f, () =>
            {
                IsBannerDisplayed = true;
                OnBannerLoaded?.Invoke();
            });
        }

        protected override void LoadInterstitialAds()
        {
            Log("Loading interstitial ads");
        }

        protected override void LoadRewardedAds()
        {
            Log("Loading rewarded ads");
            App.Core.Timer.Delay(3f, () =>
            {
                Log("Loaded rewarded ads");
                rewardAdReady = true;
            });
        }

        public override bool IsRewardAdReady()
        {
            return rewardAdReady;
        }
    }
}

