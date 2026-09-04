namespace Core.SaveSystemBase
{
    public interface ISaveabale<TSaveData>
    {
        TSaveData Capture();
        void Restore(TSaveData restoredData);
    }
}