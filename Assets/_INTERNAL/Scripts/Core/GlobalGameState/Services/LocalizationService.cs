using R3;
using System;
using UnityEngine;
#if UNITY_WEBGL
using YG;
#endif

namespace Core.GlobalGameState.Services
{
    public sealed class LocalizationService : IDisposable
    {
        private readonly Subject<SystemLanguage> _languageChangedSignal = new();

        public SystemLanguage CurrentLanguage { get; private set; } = Application.systemLanguage;
        public Observable<SystemLanguage> LanguageChangedSignal => _languageChangedSignal.AsObservable();

        public LocalizationService()
        {
#if UNITY_WEBGL
        YG2.onSwitchLang += OnYgLangChanged;
        YG2.GetLanguage();
        OnYgLangChanged(YG2.lang);
#endif
        }

        public void Dispose()
        {
#if UNITY_WEBGL
        YG2.onSwitchLang -= OnYgLangChanged;
#endif
        }

#if UNITY_WEBGL
        private void OnYgLangChanged(string code)
        {
            var mapped = Map(code);
            if (mapped == CurrentLanguage) return;

            CurrentLanguage = mapped;
            _languageChangedSignal.OnNext(CurrentLanguage);
        }


        private static SystemLanguage Map(string code) => code?.ToLower() switch
        {
            "ru" => SystemLanguage.Russian,
            "en" => SystemLanguage.English,
            _ => SystemLanguage.Russian
        };
#endif
    }
}