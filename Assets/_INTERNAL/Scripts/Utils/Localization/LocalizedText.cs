using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils.Localization
{
    public sealed class LocalizedText : IEquatable<LocalizedText>
    {
        private readonly IReadOnlyDictionary<SystemLanguage, string> _texts;

        public LocalizedText(Dictionary<SystemLanguage, string> texts)
        {
            if (texts == null || texts.Count == 0)
                throw new ArgumentException("[Localized Text] Localized Text cannot be empty");

            _texts = new Dictionary<SystemLanguage, string>(texts);
        }

        public string Get(SystemLanguage language)
        {
            if (_texts.TryGetValue(language, out var text))
                return text;

            return _texts.Values.First();
        }

        public bool Equals(LocalizedText other)
        {
            if (other == null)
                return false;
            if (_texts.Count != other._texts.Count)
                return false;

            return !_texts.Except(other._texts).Any();
        }

        public override bool Equals(object obj) => Equals(obj as LocalizedText);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                foreach (var pair in _texts.OrderBy(x => x.Key))
                {
                    hash = hash * 31 + pair.Key.GetHashCode();
                    hash = hash * 31 + pair.Value.GetHashCode();
                }

                return hash;
            }
        }
    }
}