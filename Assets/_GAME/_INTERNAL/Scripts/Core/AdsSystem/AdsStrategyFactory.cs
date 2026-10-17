#if UNITY_ANDROID
using Core.AdsSystem.Mobile;
using SO.AdsConfigs;
#elif UNITY_WEBGL
using Core.AdsSystem.Web;
#endif

namespace Core.AdsSystem
{
    public static class AdsStrategyFactory
    {
        public static IAdsStrategy CreateStrategy()
        {
#if UNITY_WEBGL
            return new WebAdsProvider();
#else
            throw new System.PlatformNotSupportedException("Platform not supported");
#endif
        }

#if UNITY_ANDROID
        public static IAdsStrategy CreateStrategy(RewardAdsConfig config)
        {
            return new MobileAdsProvider(config.PlacementsDict);
        }
#endif
    }
}