using Entry.Local.Gameplay;

namespace Core.AddressablesLoadSystem
{
    public static class GameplayLoaderFactory
    {
        public static IGameplayResourceLoader Create()
        {
#if USE_ADDRESSABLES
            return new GameplayAddressablesResourceLoader();
#else
            return new GameplayResourceLoader();
#endif
        }
    }
}