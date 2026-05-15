namespace Core.SaveSystemBase.Data
{
    [System.Serializable]
    public class ItemUpgradeData
    {
        public const int CurrentContentRevision = 6;

        public int ContentRevision = CurrentContentRevision;

        public string Name;
        public string Description;

        public int ID;
        public string StableId;
        public int Level;

        public bool IsOpened;

        public float Price;
        public float UpgradeAmount;
    }
}