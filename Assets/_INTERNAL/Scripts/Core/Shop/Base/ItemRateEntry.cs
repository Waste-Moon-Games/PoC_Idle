using UnityEngine;

namespace Core.Shop.Base
{
    [System.Serializable]
    public class ItemRateEntry
    {
        [field: SerializeField] public string ItemID {  get; set; }
        [field: SerializeField] public float PriceRate { get; set; } = 1f;
        [field: SerializeField] public float BonusRate { get; set; } = 1f;
    }
}