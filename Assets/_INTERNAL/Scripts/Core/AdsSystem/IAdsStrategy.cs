using Cysharp.Threading.Tasks;
using System;

namespace Core.AdsSystem
{
    public interface IAdsStrategy
    {
        UniTask SetupLoaders();
        void ShowInterstitial();
        void ShowRewarded(Action onComplete = null);
    }
}