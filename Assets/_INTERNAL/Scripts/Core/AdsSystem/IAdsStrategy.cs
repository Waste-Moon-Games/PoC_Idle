using System;

namespace Core.AdsSystem
{
    public interface IAdsStrategy
    {
        void SetupLoaders();
        void ShowInterstitial();
        void ShowRewarded(Action onComplete = null);
    }
}