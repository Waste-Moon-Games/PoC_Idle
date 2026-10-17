#if UNITY_ANDROID
using Core.Services.Analytics;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace Core.AdsSystem.Mobile
{
    public class MobileAdsProvider : IAdsStrategy
    {
        private string _requestedRewardedID = string.Empty;

        private readonly Dictionary<RewardedAdType, string> _placementsDict;

        private RewardedAdLoader _rewardedLoader;
        private RewardedAd _rewardedAd;
        private RewardedAdType _type;

        private InterstitialAdLoader _interstitialLoader;
        private Interstitial _interstitialAd;

        private bool _isRewardedProcessing;

        private event Action _onComplete;

        public MobileAdsProvider(Dictionary<RewardedAdType, string> placementsDict)
        {
            _placementsDict = new();

            foreach (var kpv in placementsDict.Keys)
                _placementsDict[kpv] = placementsDict[kpv];

            SetupLoaders().Forget();
        }

        public async UniTask SetupLoaders()
        {
            _rewardedLoader = new();

            _interstitialLoader = new();

            await RequestInterstitial();
            await RequestRewarded(RewardedAdType.Double_Offline_Income);
        }

        public void ShowInterstitial() => _interstitialAd?.Show();

        public void ShowRewarded(RewardedAdType type, Action onComplete = null)
        {
            if(_isRewardedProcessing)
                return;

            _isRewardedProcessing = true;
            _onComplete = onComplete;
            _type = type;

            if (_rewardedAd != null)
                _rewardedAd.Show();
            else
                RequestRewarded(type).Forget();
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

        private async UniTask RequestRewarded(RewardedAdType type)
        {
            string adUnitId;

            if(_placementsDict.TryGetValue(type, out string placementId))
                adUnitId = placementId;
            else
            {
                Debug.LogWarning($"[Mobile Ads Provider] Placement by type '{type}' not found!");
                return;
            }

            _requestedRewardedID = adUnitId;

            AnalyticsService.Instance.ReportRewardedAdClick(adUnitId);

            try
            {
                var loadedAd = await _rewardedLoader.LoadAd(new(adUnitId));
                HandleRewardedAdLoaded(loadedAd);
            }
            catch (OperationCanceledException) { }
            catch (AdLoadingException e)
            {
                HandleRewardedFailedToLoad(e);
            }
        }

        private async UniTask RequestInterstitial()
        {
            string adUnitId;

            if(_placementsDict.TryGetValue(RewardedAdType.Transition_Interstitial, out string placementId))
                adUnitId = placementId;
            else
            {
                Debug.LogWarning($"[Mobile Ads Provider] Placement by type '{RewardedAdType.Transition_Interstitial}' not found!");
                return;
            }

            try
            {
                var loadedAd = await _interstitialLoader.LoadAd(new(adUnitId));
                HandleInterstitialAdLoaded(loadedAd);
            }
            catch (OperationCanceledException) { }
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
            Debug.Log($"[Mobile Ads Provider] Ad '{e.GetInfo().AdUnitId}' loaded.");

            _rewardedAd.OnRewarded += HandleRewarded;
            _rewardedAd.OnAdShown += HandleRewardedAdShown;
            _rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            _rewardedAd.OnAdDismissed += HandleRewardedAdDismissed;

            if(_isRewardedProcessing)
                _rewardedAd.Show();
        }

        private void HandleRewardedAdDismissed(object sender, EventArgs e)
        {
            _isRewardedProcessing = false;
            DestroyRewarded();
            RequestRewarded(_type).Forget();
        }

        private void HandleRewardedFailedToLoad(AdLoadingException e)
        {
            _isRewardedProcessing = false;

            DestroyRewarded();
            RequestRewarded(_type).Forget();

            _onComplete = null;
        }

        private void HandleInterstitialFailedToLoad(AdLoadingException e)
        {
            DestroyInterstitial();
            RequestInterstitial().Forget();
        }

        private void HandleRewardedAdFailedToShow(object sender, AdFailureEventArgs e)
        {
            _isRewardedProcessing = false;

            DestroyRewarded();
            RequestRewarded(_type).Forget();
        }

        private void HandleRewardedAdShown(object sender, EventArgs e)
        {
            Debug.Log($"YandexAds: ad shown {e}");
        }

        private void HandleRewarded(object sender, Reward e)
        {
            _isRewardedProcessing = false;
            
            _onComplete?.Invoke();
            _onComplete = null;

            AnalyticsService.Instance.ReportRewardedAdReward(_requestedRewardedID);
            _requestedRewardedID = string.Empty;
        }
    }
}
#endif