#if UNITY_ANDROID
using Cysharp.Threading.Tasks;
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
            SetupLoaders().Forget();
        }

        public async UniTask SetupLoaders()
        {
            _rewardedLoader = new();

            _interstitialLoader = new();

            await RequestRewarded();
            await RequestInterstitial();
        }

        public void ShowInterstitial() => _interstitialAd?.Show();

        public void ShowRewarded(Action onComplete = null)
        {
            _onComplete = onComplete;

            if (_rewardedAd != null)
            {
                _rewardedAd.Show();
            }
            else
            {
                _pendingShowRewarded = true;
                RequestRewarded().Forget();
            }
        }

        private void DestroyRewarded()
        {
            _rewardedAd?.Destroy();
            _rewardedAd = null;
        }

        private void DestroyInterstitial()
        {
            _interstitialAd?.Destroy();
            _interstitialAd = null;
        }

        private async UniTask RequestRewarded()
        {
            string adUnitId = "R-M-19182918-1";
            try
            {
                var loadedAd = await _rewardedLoader.LoadAd(new(adUnitId));
                HandleRewardedAdLoaded(loadedAd);
            }
            catch (AdLoadingException e)
            {
                HandleRewardedFailedToLoad(e);
            }
            
            
        }

        private async UniTask RequestInterstitial()
        {
            string adUnitId = "R-M-19182918-2";
            try
            {
                var loadedAd = await _interstitialLoader.LoadAd(new(adUnitId));
                HandleInterstitialAdLoaded(loadedAd);
            }
            catch (AdLoadingException e)
            {
                HandleInterstitialFailedToLoad(e);
            }
        }

        private void HandleInterstitialAdLoaded(Interstitial e)
        {
            _interstitialAd = e;

            _interstitialAd.OnAdClicked += HandleInterstitialAdClicked;
            _interstitialAd.OnAdShown += HandleInterstitialAdShown;
            _interstitialAd.OnAdFailedToShow += HandleInterstitialAdFailedToShow;
            _interstitialAd.OnAdDismissed += HandleInterstitialAdDismissed;
        }

        private void HandleInterstitialAdClicked(object sender, EventArgs e) { }

        private void HandleInterstitialAdShown(object sender, EventArgs e)
        {
            DestroyInterstitial();
            RequestInterstitial().Forget();
        }

        private void HandleInterstitialAdFailedToShow(object sender, EventArgs e)
        {
            DestroyInterstitial();
            RequestInterstitial().Forget();
        }

        private void HandleInterstitialAdDismissed(object sender, EventArgs e)
        {
            DestroyInterstitial();
            RequestInterstitial().Forget();
        }

        private void HandleRewardedAdLoaded(RewardedAd e)
        {
            _rewardedAd = e;

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
            RequestRewarded().Forget();
        }

        private void HandleRewardedFailedToLoad(AdLoadingException e)
        {
            DestroyRewarded();
            RequestRewarded().Forget();

            _onComplete = null;
        }

        private void HandleInterstitialFailedToLoad(AdLoadingException e)
        {
            DestroyInterstitial();
            RequestInterstitial().Forget();
        }

        private void HandleRewardedAdFailedToShow(object sender, AdFailureEventArgs e)
        {
            DestroyRewarded();
            RequestRewarded().Forget();
        }

        private void HandleRewardedAdShown(object sender, EventArgs e)
        {
            Debug.Log($"YandexAds: ad shown {e}");
        }

        private void HandleRewarded(object sender, Reward e)
        {
            _onComplete?.Invoke();
            _onComplete = null;
        }
    }
}
#endif