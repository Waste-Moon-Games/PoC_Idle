#if UNITY_WEBGL
using System;
using UnityEngine;
using YG;

namespace Core.AdsSystem.Web
{
    public class WebAdsProvider : IAdsStrategy
    {
        private Action _pendingCallback;

        public WebAdsProvider()
        {
            try
            {
                YG2.onRewardAdv += OnYGReward;
            }
            catch
            {
                Debug.LogWarning("YG2 SDK event not found. Make sure YG2 is present in WebGL build.");
            }
        }

        public void SetupLoaders() { }

        public void ShowInterstitial()
        {
            try
            {
                YG2.InterstitialAdvShow();
            }
            catch (Exception)
            {
                Debug.LogWarning("YG2.InterstitialAdvShow failed or YG2 not present.");
            }
        }

        public void ShowRewarded(Action onComplete = null)
        {
            _pendingCallback = onComplete;

            try
            {
                YG2.RewardedAdvShow("");
            }
            catch (Exception)
            {
                Debug.LogWarning("YG2.RewVideoShow call failed. Invoking callback immediately as fallback.");
                _pendingCallback?.Invoke();
                _pendingCallback = null;
            }
        }

        private void OnYGReward(string id)
        {
            _pendingCallback?.Invoke();
            _pendingCallback = null;
        }
    }
}
#endif