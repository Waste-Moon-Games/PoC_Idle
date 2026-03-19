using Core.SaveSystemBase;
using Core.SaveSystemBase.Data;
using UnityEngine;
using YG;

namespace Core.SaveSystem.Web
{
    public class YandexSaveSystemStrategy : ISaveSystemStrategy
    {
        public void Save(PlayerData data, string key)
        {
            YG2.saves.JsonData = JsonUtility.ToJson(data);
            YG2.SaveProgress();

            Debug.Log($"[Yandex Save System Strategy] Game saved");
        }

        public PlayerData Load(string key)
        {
            PlayerData result = JsonUtility.FromJson<PlayerData>(YG2.saves.JsonData);
            Debug.Log($"[Yandex Save System Strategy] Game loaded");

            return result;
        }

        public void Delete(string key)
        {
            YG2.SetDefaultSaves();
        }
    }
}