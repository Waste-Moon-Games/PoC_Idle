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
            YG2.saves.PlayerData = data;
            YG2.SaveProgress();

            Debug.Log($"[Yandex Save System Strategy] Game saved");
        }

        public PlayerData Load(string key)
        {
            Debug.Log($"[Yandex Save System Strategy] Game loaded");
            return YG2.saves.PlayerData;
        }

        public void Delete(string key)
        {
            YG2.SetDefaultSaves();
        }
    }
}