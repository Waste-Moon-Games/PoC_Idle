using System;
using Random = UnityEngine.Random;

namespace Core.AdsSystem
{
    public class AdsSystemContext
    {
        private readonly IAdsStrategy _currentStrategy;
        private readonly float _interstitialAdShowChance = 1f;

        public AdsSystemContext(IAdsStrategy adsStrategy, float interstitialAdShowChance)
        {
            _currentStrategy = adsStrategy;
            _interstitialAdShowChance = interstitialAdShowChance;
        }

        public void ShowRewarded(Action onCompeted = null) => _currentStrategy.ShowRewarded(onCompeted);

        public void ShowInterstitial()
        {
            float randRoll = Random.Range(0f, 1f);

            if(randRoll < _interstitialAdShowChance)
                _currentStrategy.ShowInterstitial();
        }
    }
}
