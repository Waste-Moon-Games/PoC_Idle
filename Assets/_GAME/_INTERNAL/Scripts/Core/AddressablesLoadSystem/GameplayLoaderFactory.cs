using Entry.Local.Gameplay;

namespace Core.AddressablesLoadSystem
{
    public static class GameplayLoaderFactory
    {
        public static IUniversalResourceLoader Create()
        {
#if USE_ADDRESSABLES
            return new GameplayAddressablesResourceLoader();
#else
            return new GameplayResourceLoader();
#endif
        }
    }
}