using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Localization
{
    [Serializable]
    public sealed class LocalizedText : ISerializationCallbackReceiver
    {
        [Serializable]
        private struct Entry
        {
            public SystemLanguage Language;
            [TextArea] public string Text;
        }

        [SerializeField] private List<Entry> _entries = new();

        private Dictionary<SystemLanguage, string> _cache;

        public string Get(SystemLanguage language)
        {
            EnsureCache();

            if (_cache.TryGetValue(language, out var text))
                return text;

            if (_cache.TryGetValue(SystemLanguage.English, out var en))
                return en;

            foreach (var pair in _cache)
                return pair.Value;

            return string.Empty;
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            RebuildCache();
        }

        private void EnsureCache()
        {
            if (_cache == null)
                RebuildCache();
        }

        private void RebuildCache()
        {
            _cache = new Dictionary<SystemLanguage, string>();

            for (int i = 0; i < _entries.Count; i++)
            {
                var e = _entries[i];
                if (string.IsNullOrWhiteSpace(e.Text))
                    continue;

                if (!_cache.ContainsKey(e.Language))
                    _cache.Add(e.Language, e.Text);
            }
        }
    }
}