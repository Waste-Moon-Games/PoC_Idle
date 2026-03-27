#if UNITY_ANDROID
using Core.AdsSystem.Mobile;
#endif

#if UNITY_WEBGL
using Core.AdsSystem.Web;
#endif

namespace Core.AdsSystem
{
    public static class AdsStrategyFactory
    {
        public static IAdsStrategy CreateStrategy()
        {
#if UNITY_ANDROID
            return new MobileAdsProvider();
#endif
#if UNITY_WEBGL
            return new WebAdsProvider();
#endif
        }
    }
}