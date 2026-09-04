using Core.SaveSystemBase.Data;

namespace Core.SaveSystemBase
{
    public class SaveSystemContext
    {
        private readonly ISaveSystemStrategy _currentStrategy;

        public SaveSystemContext(ISaveSystemStrategy saveSystemStrategy) => _currentStrategy = saveSystemStrategy;

        public void Save(PlayerData data, string key)
        {
            if (_currentStrategy == null)
                return;
            _currentStrategy.Save(data, key);
        }

        public PlayerData Load(string key)
        {
            if (_currentStrategy == null)
                return new();

            return _currentStrategy.Load(key);
        }

        public void Delete(string key)
        {
            if (_currentStrategy == null)
                return;

            _currentStrategy.Delete(key);
        }
    }
}