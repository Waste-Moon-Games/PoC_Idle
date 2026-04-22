#if UNITY_ANDROID
using Core.AdsSystem.Mobile;
#elif UNITY_WEBGL
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
#elif UNITY_WEBGL
            return new WebAdsProvider();
#else
            throw new System.PlatformNotSupportedException("Platform not supported");
#endif
        }
    }
}