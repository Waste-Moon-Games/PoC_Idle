using System.Collections.Generic;

namespace Core.SaveSystemBase
{
    public interface ISaveSystemStrategy
    {
        void Save<T>(T data, string key);
        T Load<T>(string key, T defaultValue);
        void Delete(string key);
    }
}