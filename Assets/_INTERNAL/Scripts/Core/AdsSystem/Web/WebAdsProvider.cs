#if UNITY_WEBGL
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YG;

namespace Core.AdsSystem.Web
{
    public class WebAdsProvider : IAdsStrategy
    {
        private Action _pendingCallback;

        public UniTask SetupLoaders() => UniTask.CompletedTask;

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

        public void ShowRewarded(RewardedAdType type, Action onComplete = null)
        {
            _pendingCallback = onComplete;
            
            string adId = string.Empty;

            if(type == RewardedAdType.Free_Gems)
                adId = "Free_Gems";
            else
                adId = "Income_Boost";

            try
            {
                YG2.RewardedAdvShow(adId, () =>
                {
                    _pendingCallback?.Invoke();
                    _pendingCallback = null;
                });
            }
            catch (Exception)
            {
                Debug.LogWarning("YG2.RewVideoShow call failed. Invoking callback immediately as fallback.");
                _pendingCallback = null;
            }
        }
    }
}
#endif