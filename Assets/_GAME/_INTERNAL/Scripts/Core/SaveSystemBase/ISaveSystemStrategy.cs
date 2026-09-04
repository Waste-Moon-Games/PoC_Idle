using Core.SaveSystemBase.Data;

namespace Core.SaveSystemBase
{
    public interface ISaveSystemStrategy
    {
        void Save(PlayerData data, string key);
        PlayerData Load(string key);
        void Delete(string key);
    }
}