using Core.SaveSystemBase;
using Core.SaveSystemBase.Data;
using UnityEngine;

namespace Core.SaveSystem.Mobile
{
    public class MobileSaveSystemStrategy : ISaveSystemStrategy
    {
        public void Save(PlayerData data, string key)
        {
            PlayerPrefs.SetString(key, JsonUtility.ToJson(data));
            Debug.Log($"[Mobile Save System Strategy] Game saved");
        }

        public PlayerData Load(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                Debug.Log($"[Mobile Save System Strategy] Game saved");
                return JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(key));
            }
            return new();
        }

        public void Delete(string key) => PlayerPrefs.DeleteKey(key);
    }
}