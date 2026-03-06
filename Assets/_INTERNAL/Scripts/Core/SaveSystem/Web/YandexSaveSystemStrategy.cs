using Core.SaveSystemBase;

namespace Core.SaveSystem.Web
{
    public class YandexSaveSystemStrategy : ISaveSystemStrategy
    {
        public void Save<T>(T data, string key)
        {
        }

        public T Load<T>(string key, T defaultValue)
        {
            return defaultValue;
        }

        public void Delete(string key)
        {
        }
    }
}