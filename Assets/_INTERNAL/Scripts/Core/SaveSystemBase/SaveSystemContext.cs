using System;
using System.Linq;
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

            string typeName = isWebGL
                ? "Core.SaveSystem.Web.YandexSaveSystemStrategy"
                : "Core.SaveSystem.Mobile.MobileSaveSystemStrategy";

            string assemblyName = isWebGL
                ? "Core.SaveSystem.Web"
                : "Core.SaveSystem.Mobile";

            string fullTypeName = $"{typeName},{assemblyName}";

            Debug.Log($"[Save System Context] Attempting to load strategy: {fullTypeName}");

            Type strategyType = Type.GetType(fullTypeName)
                ?? AppDomain.CurrentDomain
                    .GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == assemblyName)
                    ?.GetType(typeName);

            if (strategyType != null)
            {
                _currentStrategy = (ISaveSystemStrategy)Activator.CreateInstance(strategyType);
                Debug.Log($"[Save System Context] SUCCESS: Strategy loaded for {(isWebGL ? "WebGL" : "Mobile")}");
                return;
            }

            Debug.LogWarning($"[Save System Context] FAILED: Could not find type {fullTypeName}.");
            throw new Exception($"Save System Strategy not found: {fullTypeName}. Check Assembly Definition/stripping settings.");
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
