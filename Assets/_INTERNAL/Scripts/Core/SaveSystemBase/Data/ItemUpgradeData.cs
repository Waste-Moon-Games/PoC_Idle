namespace Core.SaveSystemBase.Data
{
    [System.Serializable]
    public class ItemUpgradeData
    {
        public const int CurrentContentRevision = 3;

        public int ContentRevision = CurrentContentRevision;

        public string Name;
        public string Description;

        public int ID;
        public int Level;

        public bool IsOpened;

        public float Price;
        public float UpgradeAmount;
    }
}