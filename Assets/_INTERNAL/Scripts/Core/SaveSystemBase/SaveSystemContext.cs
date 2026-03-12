namespace Core.SaveSystemBase
{
    public class SaveSystemContext
    {
        private readonly ISaveSystemStrategy _currentStrategy;

        public SaveSystemContext(ISaveSystemStrategy saveSystemStrategy) => _currentStrategy = saveSystemStrategy;

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