using System;
using UnityEngine;

namespace Core.SaveSystemBase
{
    public class SaveSystemContext
    {
        private ISaveSystemStrategy _currentStrategy;

        public SaveSystemContext() => InitializeStrategy();

        private void InitializeStrategy()
        {
            bool isWebGL = Application.platform == RuntimePlatform.WebGLPlayer;

            Type strategyType = null;
            string assemblyName = string.Empty;
            string typeName = string.Empty;

            if (isWebGL)
            {
                typeName = "Core.SaveSystem.Web.YandexSaveSystemStrategy";
                assemblyName = "Core.SaveSystem.Web";
            }
            else
            {
                typeName = "Core.SaveSystem.Mobile.MobileSaveSystemStrategy";
                assemblyName = "Core.SaveSystem.Mobile";
            }

            string fullTypeName = $"{typeName},{assemblyName}";

            Debug.Log($"[Save System Context] Attempting to load strategy: {fullTypeName}");

            strategyType = Type.GetType(fullTypeName);
            if(strategyType != null)
            {
                _currentStrategy = (ISaveSystemStrategy)Activator.CreateInstance(strategyType);
                Debug.Log($"[Save System Context] SUCCESS: Strategy loaded for {(isWebGL ? "WebGL" : "Mobile")}");
            }
            else
            {
                Debug.LogWarning($"[Save System Context] FAILED: Could not find type {fullTypeName}. Falling back to Player Prefs (or dummy).");
                throw new Exception($"Save System Strategy not found: {fullTypeName}. Check Assembly Definition");
            }
        }

        public void Save<T>(T data, string key)
        {
            if (_currentStrategy == null) 
                return;
            _currentStrategy.Save(data, key);
        }

        public T Load<T>(string key, T defaultValue)
        {
            if (_currentStrategy == null)
                return defaultValue;

            return _currentStrategy.Load<T>(key, defaultValue);
        }

        public void Delete(string key)
        {
            if (_currentStrategy == null)
                return;

            _currentStrategy.Delete(key);
        }
    }
}