using Core.SaveSystemBase.Data;

namespace Core.GlobalGameState.Services
{
    public class SaveMigrationService
    {
        public void MigrateLoadedPlayerData(PlayerData loadedData)
        {
            if (loadedData == null)
                return;

            if(loadedData.SaveVersion < PlayerData.CurrentSaveVersion)
                loadedData.SaveVersion = PlayerData.CurrentSaveVersion;
        }
    }
}