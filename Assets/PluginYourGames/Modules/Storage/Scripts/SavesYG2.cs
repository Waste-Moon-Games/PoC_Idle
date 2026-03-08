using Core.SaveSystemBase.Data;
using UnityEngine;

namespace YG
{
    [System.Serializable]
    public partial class SavesYG
    {
        public int idSave;

        private string _jsonData = string.Empty;

        public string JsonData => _jsonData;

        public void SetData(string data)
        {
            _jsonData = data;
        }

        public string GetData()
        {
            if (string.IsNullOrEmpty(_jsonData))
                return JsonUtility.ToJson(new PlayerData());

            return _jsonData;
        }
    }
}
