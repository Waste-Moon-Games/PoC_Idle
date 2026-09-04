using Core.SaveSystemBase;
using Core.SaveSystemBase.Data;
using Newtonsoft.Json;
using UnityEngine;
using YG;

namespace Core.SaveSystem.Web
{
    public class YandexSaveSystemStrategy : ISaveSystemStrategy
    {
        public void Save(PlayerData data, string key)
        {
            YG2.saves.JsonData = JsonConvert.SerializeObject(data);
            YG2.SaveProgress();
#if UNITY_EDITOR
            Debug.Log($"[Yandex Save System Strategy] Game saved");
#endif
        }

        public PlayerData Load(string key)
        {
            PlayerData result = JsonConvert.DeserializeObject<PlayerData>(YG2.saves.JsonData);
#if UNITY_EDITOR
            Debug.Log($"[Yandex Save System Strategy] Game loaded");
#endif

            return result;
        }

        public void Delete(string key)
        {
            YG2.SetDefaultSaves();
        }
    }
}