using R3;
using System.Collections.Generic;
using UnityEngine;

namespace Core.AudioSystemCommon
{
    public static class AudioEventBus
    {
        private static readonly Subject<string> _soundPlayByIdSignal = new();

        private readonly static Dictionary<SoundType, string> _soundIdsByType = new();

        public static Observable<string> SoundPlayById => _soundPlayByIdSignal.AsObservable();

        public static void InitSoundCollection(IReadOnlyList<Sound> sounds)
        {
            for (int i = 0; i < sounds.Count; i++)
            {
                var soundData = sounds[i];
                _soundIdsByType[soundData.Type] = soundData.ID;
            }
        }

        public static void InvokeSoundSignalByType(SoundType type)
        {
            try
            {
                _soundIdsByType.TryGetValue(type, out string soundId);
                _soundPlayByIdSignal.OnNext(soundId);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"GameEntry failed: {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}