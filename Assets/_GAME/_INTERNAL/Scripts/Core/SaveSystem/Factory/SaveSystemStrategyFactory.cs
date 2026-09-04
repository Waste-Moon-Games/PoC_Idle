#if UNITY_WEBGL
using Core.SaveSystem.Web;
#else
using Core.SaveSystem.Mobile;
#endif
using Core.SaveSystemBase;

public static class SaveSystemStrategyFactory
{
    public static ISaveSystemStrategy CreateStrategy()
    {
#if UNITY_WEBGL
        return new YandexSaveSystemStrategy();
#else
        return new MobileSaveSystemStrategy();
#endif
    }
}