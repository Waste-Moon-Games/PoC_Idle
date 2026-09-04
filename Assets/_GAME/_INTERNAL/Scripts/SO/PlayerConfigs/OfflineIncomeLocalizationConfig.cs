using UnityEngine;
using Utils.Localization;

namespace SO.PlayerConfigs
{
    [CreateAssetMenu(menuName = "Configs/Localization/Offline Income", fileName = "OfflineIncomeLocalizationConfig")]
    public class OfflineIncomeLocalizationConfig : ScriptableObject
    {
        [field: SerializeField] public LocalizedText StartMessageLocalizations { get; private set; }
        [field: SerializeField] public LocalizedText OfflineIncomeLocalizations { get; private set; } 
    }
}