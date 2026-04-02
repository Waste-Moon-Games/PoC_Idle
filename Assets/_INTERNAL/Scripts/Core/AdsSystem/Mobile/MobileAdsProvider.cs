#if UNITY_ANDROID
using System;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace Core.AdsSystem.Mobile
{
    public class MobileAdsProvider : IAdsStrategy
    {
        private RewardedAdLoader _rewardedLoader;
        private RewardedAd _rewardedAd;

        private InterstitialAdLoader _interstitialLoader;
        private Interstitial _interstitialAd;

        private bool _pendingShowRewarded;

        private event Action _onComplete;

        public MobileAdsProvider()
        {
            SetupLoaders();
        }

        public void SetupLoaders()
        {
            _rewardedLoader = new RewardedAdLoader();
            _rewardedLoader.OnAdLoaded += HandleRewardedAdLoaded;
            _rewardedLoader.OnAdFailedToLoad += HandleRewardedFailedToLoad;

            _interstitialLoader = new();
            _interstitialLoader.OnAdLoaded += HandleInterstitialAdLoaded;
            _interstitialLoader.OnAdFailedToLoad += HandleInterstitialFailedToLoad;

            RequestRewarded();
            RequestInterstitial();
        }

        public void ShowInterstitial() => _interstitialAd?.Show();

        public void ShowRewarded(Action onComplete = null)
        {
            _onComplete = onComplete;

            if (_rewardedAd != null)
            {
                Debug.Log("Showing rewarded ad");
                _rewardedAd.Show();
            }
            else
            {
                Debug.LogWarning("Rewarded ad not loaded yet, will show after load");
                _pendingShowRewarded = true;
                RequestRewarded();
            }
        }

        private void DestroyRewarded()
        {
            _rewardedAd?.Destroy();
            _rewardedAd = null;
        }

        private void DestroyInterstitial()
        {
            Debug.Log("[Mobile Ads Provider] Interstitial destroyed");
            _interstitialAd?.Destroy();
            _interstitialAd = null;
        }

        private void RequestRewarded()
        {
            string adUnitId = "demo-rewarded-yandex";
            AdRequestConfiguration adRequestConfiguration = new AdRequestConfiguration.Builder(adUnitId).Build();
            _rewardedLoader.LoadAd(adRequestConfiguration);

            Debug.Log("[Mobile Ads Provider] Rewarded requested");
        }

        private void RequestInterstitial()
        {
            string adUnitId = "demo-interstitial-yandex";
            AdRequestConfiguration adRequestConfiguration = new AdRequestConfiguration.Builder(adUnitId).Build();
            _interstitialLoader.LoadAd(adRequestConfiguration);

            Debug.Log("[Mobile Ads Provider] Interstitial requested");
        }

        private void HandleInterstitialAdLoaded(object sender, InterstitialAdLoadedEventArgs e)
        {
            _interstitialAd = e.Interstitial;
            Debug.Log("YandexAds OnAdLoaded");

            _interstitialAd.OnAdClicked += HandleInterstitialAdClicked;
            _interstitialAd.OnAdShown += HandleInterstitialAdShown;
            _interstitialAd.OnAdFailedToShow += HandleInterstitialAdFailedToShow;
            _interstitialAd.OnAdDismissed += HandleInterstitialAdDismissed;
        }

        private void HandleInterstitialAdClicked(object sender, EventArgs e) { }

        private void HandleInterstitialAdShown(object sender, EventArgs e)
        {
            DestroyInterstitial();
            RequestInterstitial();
        }

        private void HandleInterstitialAdFailedToShow(object sender, EventArgs e)
        {
            DestroyInterstitial();
            RequestInterstitial();
        }

        private void HandleInterstitialAdDismissed(object sender, EventArgs e)
        {
            DestroyInterstitial();
            RequestInterstitial();
        }

        private void HandleRewardedAdLoaded(object sender, RewardedAdLoadedEventArgs e)
        {
            _rewardedAd = e.RewardedAd;
            Debug.Log($"YandexAds: ad loaded {e}");

            _rewardedAd.OnRewarded += HandleRewarded;
            _rewardedAd.OnAdShown += HandleRewardedAdShown;
            _rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            _rewardedAd.OnAdDismissed += HandleRewardedAdDismissed;

            if (_pendingShowRewarded)
            {
                _pendingShowRewarded = false;
                ShowRewarded(_onComplete);
            }
        }

        private void HandleRewardedAdDismissed(object sender, EventArgs e)
        {
            DestroyRewarded();
            RequestRewarded();
        }

        private void HandleRewardedFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            DestroyRewarded();
            RequestRewarded();

            _onComplete = null;
        }

        private void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            DestroyInterstitial();
            RequestInterstitial();
        }

        private void HandleRewardedAdFailedToShow(object sender, AdFailureEventArgs e)
        {
            DestroyRewarded();
            RequestRewarded();
        }

        private void HandleRewardedAdShown(object sender, EventArgs e)
        {
            Debug.Log($"YandexAds: ad shown {e}");
        }

        private void HandleRewarded(object sender, Reward e)
        {
            Debug.Log($"YandexAds: rewarded {e}");
            _onComplete?.Invoke();
            _onComplete = null;
        }
    }
}
#endif