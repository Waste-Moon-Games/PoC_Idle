using Core.SaveSystemBase;
using Core.SaveSystemBase.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.SaveSystem.Mobile
{
    public class MobileSaveSystemStrategy : ISaveSystemStrategy
    {
        public void Save(PlayerData data, string key)
        {
            PlayerPrefs.SetString(key, JsonConvert.SerializeObject(data));
#if UNITY_EDITOR
            Debug.Log($"[Mobile Save System Strategy] Game saved");
#endif
        }

        public PlayerData Load(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
#if UNITY_EDITOR
                Debug.Log($"[Mobile Save System Strategy] Game saved");
#endif
                return JsonConvert.DeserializeObject<PlayerData>(PlayerPrefs.GetString(key));
            }
            return new();
        }

        public void Delete(string key) => PlayerPrefs.DeleteKey(key);
    }
}