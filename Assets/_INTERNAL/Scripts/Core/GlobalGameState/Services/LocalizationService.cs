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

#if UNITY_WEBGL
        private bool _sdkReadySubscribed;
#endif

        public SystemLanguage CurrentLanguage { get; private set; } = Application.systemLanguage;
        public Observable<SystemLanguage> LanguageChangedSignal => _languageChangedSignal.AsObservable();

        public LocalizationService()
        {
#if UNITY_WEBGL
            YG2.onSwitchLang += OnYgLangChanged;

            if (YG2.isSDKEnabled)
            {
                InitializeFromYgLanguage();
            }
            else
            {
                YG2.onGetSDKData += HandleSdkReady;
                _sdkReadySubscribed = true;
            }
#endif
        }

        public void Dispose()
        {
#if UNITY_WEBGL
            YG2.onSwitchLang -= OnYgLangChanged;
            if (_sdkReadySubscribed)
            {
                YG2.onGetSDKData -= HandleSdkReady;
                _sdkReadySubscribed = false;
            }
#endif
        }

#if UNITY_WEBGL
        private void HandleSdkReady()
        {
            if (!_sdkReadySubscribed)
                return;

            YG2.onGetSDKData -= HandleSdkReady;
            _sdkReadySubscribed = false;
            InitializeFromYgLanguage();
        }

        private void InitializeFromYgLanguage()
        {
            YG2.GetLanguage();
            OnYgLangChanged(YG2.lang);
        }

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