using Core.SaveSystemBase;
using UnityEngine;
using UnityEngine.Scripting;

namespace Core.SaveSystem.Mobile
{
    [Preserve]
    public class MobileSaveSystemStrategy : ISaveSystemStrategy
    {
        public void Save<T>(T data, string key)
        {
            PlayerPrefs.SetString(key, JsonUtility.ToJson(data));
        }

        public T Load<T>(string key, T defaultValue)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return JsonUtility.FromJson<T>(PlayerPrefs.GetString(key));
            }
            return defaultValue;
        }

        public void Delete(string key) => PlayerPrefs.DeleteKey(key);
    }
}