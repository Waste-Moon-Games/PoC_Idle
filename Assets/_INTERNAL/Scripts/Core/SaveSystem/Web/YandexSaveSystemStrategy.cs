using Core.SaveSystemBase;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using YG;

namespace Core.SaveSystem.Web
{
    [Preserve]
    public class YandexSaveSystemStrategy : ISaveSystemStrategy
    {
        private Dictionary<string, string> _dataCache = new();

        public YandexSaveSystemStrategy()
        {
            YG2.onGetSDKData += OnSdkDataLoaded;
        }

        private void OnSdkDataLoaded() => ParseCurrentData();

        private void ParseCurrentData()
        {
            if (string.IsNullOrEmpty(YG2.saves.JsonData))
            {
                _dataCache.Clear();
                return;
            }

            try
            {
                var wrapper = JsonUtility.FromJson<DictionaryWrapper>(YG2.saves.JsonData);
                wrapper?.RebuildDictionary();
                _dataCache = wrapper?.Dictionary ?? new Dictionary<string, string>();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Yandex Save System Strategy] Data parsing error: {e.Message}");
                _dataCache = new();
            }
        }

        private string SerializeDictionary(Dictionary<string, string> dict)
        {
            var wrapper = new DictionaryWrapper
            {
                Keys = new(dict.Keys),
                Values = new(dict.Values)
            };

            return JsonUtility.ToJson(wrapper);
        }

        public void Save<T>(T data, string key)
        {
            string jsonData = JsonUtility.ToJson(data);

            if(_dataCache.ContainsKey(key))
                _dataCache[key] = jsonData;
            else
                _dataCache.Add(key, jsonData);

            var wrapper = new DictionaryWrapper(_dataCache);

            string fullJson = SerializeDictionary(_dataCache);

            YG2.saves.SetData(fullJson);
            YG2.SaveProgress();
        }

        public T Load<T>(string key, T defaultValue)
        {
            if (!_dataCache.ContainsKey(key))
            {
                Debug.Log($"[Yandex Save System Strategy] Key {key} not found");
                return defaultValue;
            }

            try
            {
                return JsonUtility.FromJson<T>(_dataCache[key]);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Yandex Save System Strategy] Key [{key}] deserialization error: {e.Message}");
                return defaultValue;
            }
        }

        public void Delete(string key)
        {
            if (_dataCache.Remove(key))
            {
                string fullJson = SerializeDictionary(_dataCache);
                YG2.saves.SetData(fullJson);
                YG2.SaveProgress();
            }
        }

        [System.Serializable]
        private class DictionaryWrapper
        {
            public List<string> Keys = new();
            public List<string> Values = new();

            [System.NonSerialized] public Dictionary<string, string> Dictionary = new();

            public DictionaryWrapper() { }

            public DictionaryWrapper(Dictionary<string, string> dict)
            {
                Keys = new(dict.Keys);
                Values = new(dict.Values);

                Dictionary = dict;
            }

            public void RebuildDictionary()
            {
                Dictionary = new();
                int pairCount = Mathf.Min(Keys.Count, Values.Count);
                for (int i = 0; i < pairCount; i++)
                {
                    Dictionary[Keys[i]] = Values[i];
                }
            }
        }
    }
}