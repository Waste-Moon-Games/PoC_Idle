using UnityEngine;

namespace Core.Shop.Base
{
    [System.Serializable]
    public class ItemModelConfig
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public float Price { get; private set; }
        [field: SerializeField] public float UpgradeAmount { get; private set; }
        [field: SerializeField] public int Level { get; private set; }
        [field: SerializeField] public bool IsOpened {  get; private set; }
        [field: SerializeField] public ItemType Type { get; private set; }
    }
}