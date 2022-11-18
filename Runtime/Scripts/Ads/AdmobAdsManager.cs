#if ENABLE_ADMOB
using System;
using DosinisSDK.Core;
using DosinisSDK.Utils;
using GoogleMobileAds.Api;
#if UNITY_IOS
using UnityEngine;
#endif

namespace DosinisSDK.Ads
{
    // Test ad app id : ca-app-pub-3940256099942544~3347511713
    public class AdmobAdsManager : AdsManager
    {
#if UNITY_IOS
        [SerializeField] private string rewardedId_iOs = "";
        [SerializeField] private string interstitialId_iOs = "";
        [SerializeField] private string bannerId_iOs = "";
#endif
        private const int ADS_SIZE = 3;

        private readonly RewardedAd[] rewardedAds = new RewardedAd[ADS_SIZE];

        private BannerView bannerView;

        private InterstitialAd interstitial;

        private Action<bool> onRewardedAdFinished;

        private const string INTERSTITIAL_TEST_ID = "ca-app-pub-3940256099942544/1033173712";
        private const string BANNER_TEST_ID = "ca-app-pub-3940256099942544/6300978111";
        private const string REWARDED_TEST_ID = "ca-app-pub-3940256099942544/5224354917";

        private bool rewarded = false;

        public override event Action OnBannerLoaded;
        public override event Action<string> OnInterstitialShown;
        public override event Action<string> OnRewardedShown;

        protected override void OnInit(IApp app)
        {
            MobileAds.Initialize(initStatus =>
            {
                Log("Admob adapters initialized");
            });

            LoadRewardedAds();
            LoadInterstitialAds();

            if (showBanner)
            {
                LoadBanner();
                ShowBanner();
            }
        }

        private void PauseIOSGame(bool value)
        {
#if UNITY_IOS
            Time.timeScale = value ? 0 : 1;
            AudioListener.pause = value;
#endif
        }

        // Banner
        
        public override void ShowBanner()
        {
            Log($"Showing banner ad");
            
            if (bannerView != null)
            {
                bannerView.Show();
            }
        }

        public override void HideBanner()
        {
            Log($"Hiding banner ad");
            
            if (bannerView != null)
            {
                bannerView.Hide();
            }
        }

        protected override void LoadBanner()
        {
            Log($"Loading banner ad");
            
            IsBannerDisplayed = false;
            AdRequest request = new AdRequest.Builder().Build();
            
            string id = bannerId;

#if UNITY_IOS
            id = bannerId_iOs;
#endif
            
            if (useTestAds)
            {
                id = BANNER_TEST_ID;
            }
            
            if (bannerPosition == BannerPosition.Bottom)
            {
                bannerView = new BannerView(id, AdSize.Banner, AdPosition.Bottom);
            }
            else
            {
                bannerView = new BannerView(id, AdSize.Banner, AdPosition.Top);
            }
            
            bannerView.OnAdOpening += (sender,e) =>
            {
                IsBannerDisplayed = true;
            };
            
            bannerView.OnAdLoaded += (sender, e) =>
            {
                OnBannerLoaded?.Invoke();
            };

            bannerView.LoadAd(request);
        }

        // Interstitial

        protected override void LoadInterstitialAds()
        {
            string id = interstitialId;

#if UNITY_IOS
            id = interstitialId_iOs;
#endif
            
            if (useTestAds)
            {
                id = INTERSTITIAL_TEST_ID;
            }

            interstitial = new InterstitialAd(id);
            AdRequest request = new AdRequest.Builder().Build();

            interstitial.OnAdClosed += HandleInterstitialClosed;
            interstitial.OnAdOpening += HandleInterstitialOpening;
            interstitial.OnAdFailedToShow += HandleInterstitialFailedToShow;

            interstitial.LoadAd(request);
        }

        public override void ShowInterstitial(string placement = "")
        {
            Log($"Showing interstitial ad {placement}");

            if (interstitial.IsLoaded())
            {
                OnInterstitialShown?.Invoke(placement);
                interstitial.Show();
            }
        }
        
        // Rewarded

        private RewardedAd CreateAndLoadRewardedAd(string adUnitId)
        {
            RewardedAd rewardedAd = new RewardedAd(adUnitId);

            rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
            rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
            rewardedAd.OnAdOpening += HandleRewardedAdOpening;
            rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
            rewardedAd.OnAdClosed += HandleRewardedAdClosed;

            AdRequest request = new AdRequest.Builder().Build();
            rewardedAd.LoadAd(request);
            return rewardedAd;
        }

        public override bool IsRewardAdReady()
        {
            foreach (var ad in rewardedAds)
            {
                if (ad.IsLoaded())
                    return true;
            }

            return false;
        }

        protected override void LoadRewardedAds()
        {
            Log("AdManager : Loading new ads...");

            string id = rewardedId;

#if UNITY_IOS
            id = rewardedId_iOs;
#endif
            
            if (useTestAds)
            {
                id = REWARDED_TEST_ID;
            }

            for (int i = 0; i < ADS_SIZE; i++)
            {
                if (rewardedAds[i] == null || !rewardedAds[i].IsLoaded())
                {
                    rewardedAds[i] = CreateAndLoadRewardedAd(id);
                }
            }
        }

        public override void ShowRewardedAd(string placement, Action<bool> callBack)
        {
            Log($"Showing rewarded ad {placement}");

            onRewardedAdFinished = callBack;

            foreach (RewardedAd ad in rewardedAds)
            {
                if (ad != null && ad.IsLoaded())
                {
                    OnRewardedShown?.Invoke(placement);
                    ad.Show();
                    break;
                }
            }
        }

        // RewardedAd Callbacks

        private void HandleRewardedAdClosed(object sender, EventArgs e)
        {
            Dispatcher.RunOnMainThread(() =>
            {
                onRewardedAdFinished?.Invoke(rewarded);
                rewarded = false;
                PauseIOSGame(false);
                LoadRewardedAds();
            });

            Log("AdManager : Rewarded ad closed");
        }

        private void HandleUserEarnedReward(object sender, Reward e)
        {
            Log("AdManager : Rewarded ad user rewarded");
            rewarded = true;
        }

        private void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs e)
        {
            Dispatcher.RunOnMainThread(() =>
            {
                PauseIOSGame(false);
                onRewardedAdFinished?.Invoke(false);
            });

            Log($"AdManager :{e.AdError.GetMessage()}");
        }

        private void HandleRewardedAdOpening(object sender, EventArgs e)
        {
            Log("AdManager : Rewarded Ad is opening");
            PauseIOSGame(true);
        }

        private void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            Log($"AdManager :{e.LoadAdError.GetMessage()}");
        }

        private void HandleRewardedAdLoaded(object sender, EventArgs e)
        {
            Log("AdManager : Rewarded Ad is Loaded");
        }

        // Interstitial Callbacks

        private void HandleInterstitialClosed(object sender, EventArgs e)
        {
            Dispatcher.RunOnMainThread(()=>
            {
                 PauseIOSGame(false);
                 LoadInterstitialAds();
            });
        }
        
        private void HandleInterstitialOpening(object sender, EventArgs e)
        {
            Dispatcher.RunOnMainThread(() =>
            {
                PauseIOSGame(true);
            });
        }
        
        private void HandleInterstitialFailedToShow(object sender, EventArgs e)
        {
            Dispatcher.RunOnMainThread(() =>
            {
                PauseIOSGame(false);
            });
        }
    }
}
#endif