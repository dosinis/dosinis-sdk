#if ENABLE_ADMOB
using DosinisSDK.Core;
using DosinisSDK.Utils;
using GoogleMobileAds.Api;
using System;

namespace DosinisSDK.Ads
{
    public class AdmobAdManager : AdManager
    {
        private const int ADS_SIZE = 3;

        private RewardedAd[] rewardedAds = new RewardedAd[ADS_SIZE];

        private BannerView bannerView;

        private InterstitialAd interstitial;

        private Action<bool> OnRewardedAdFinished;

        private const string intesrtitialTestId = "ca-app-pub-3940256099942544/1033173712";
        private const string bannerTestId = "ca-app-pub-3940256099942544/6300978111";
        private const string rewardedTestId = "ca-app-pub-3940256099942544/5224354917";

        private bool rewarded = false;

        public override void OnInit(IApp app)
        {
            MobileAds.Initialize(initStatus =>
            {
                Log("Admob adapters initialized");
            });

            LoadRewardedAds();
            LoadInterstitialAds();

            if (showBanner)
            {
                ShowBanner();
                LoadBanner();
            }
        }

        protected override void InitializeRewardedAds()
        {

        }

        protected override void LoadBanner()
        {
            AdRequest request = new AdRequest.Builder().Build();
            bannerView.LoadAd(request);
        }

        protected override void LoadInterstitialAds()
        {
            string id = interstitialId;

            if (useTestAds)
            {
                id = intesrtitialTestId;
            }

            interstitial = new InterstitialAd(id);
            AdRequest request = new AdRequest.Builder().Build();

            interstitial.OnAdClosed += HandleInterstitialClosed;

            interstitial.LoadAd(request);
        }

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

        protected override void LoadRewardedAds()
        {
            Log("AdManager : Loading new ads...");

            string id = rewardedId;

            if (useTestAds)
            {
                id = rewardedTestId;
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

            OnRewardedAdFinished = callBack;

            foreach (RewardedAd ad in rewardedAds)
            {
                if (ad != null && ad.IsLoaded())
                {
                    ad.Show();
                    break;
                }
            }
        }

        public override void ShowBanner(string placement = "")
        {
            Log($"Showing banner ad {placement}");

            string id = bannerId;

            if (useTestAds)
            {
                id = bannerTestId;
            }

            if (bannerPosition == BannerPosition.Bottom)
            {
                bannerView = new BannerView(id, AdSize.Banner, AdPosition.Bottom);
            }
            else
            {
                bannerView = new BannerView(id, AdSize.Banner, AdPosition.Top);
            }
        }

        public override void ShowInterstitial(string placement = "")
        {
            Log($"Showing interstitial ad {placement}");

            if (interstitial.IsLoaded())
            {
                interstitial.Show();
            }
        }

        // Rewarded Ads

        private void HandleRewardedAdClosed(object sender, EventArgs e)
        {
            Dispatcher.RunOnMainThread(() =>
            {
                OnRewardedAdFinished?.Invoke(rewarded);
                rewarded = false;
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
                OnRewardedAdFinished?.Invoke(false);
            });

            Log($"AdManager :{e.AdError.GetMessage()}");
        }

        private void HandleRewardedAdOpening(object sender, EventArgs e)
        {
            Log("AdManager : Rewarded Ad is opening");
        }

        private void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            Log($"AdManager :{e.LoadAdError.GetMessage()}");
        }

        private void HandleRewardedAdLoaded(object sender, EventArgs e)
        {
            Log("AdManager : Rewarded Ad is Loaded");
        }

        // Interstitial

        private void HandleInterstitialClosed(object sender, EventArgs e)
        {
            Dispatcher.RunOnMainThread(() =>
            {
                LoadInterstitialAds();
            });
        }
    }
}
#endif