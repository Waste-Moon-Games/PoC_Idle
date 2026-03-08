using Core.Consts;

namespace Core.SaveSystemBase.Data
{
    [System.Serializable]
    public class RewardData
    {
        public string ID = "Reward_ID";

        public int RequiredLevel = 0;
        public float RewardAmount = 0f;

        public RewardType Type;
        public RewardState State;

        public bool Received = false;

        public RewardType RewardType { get; set; }
    }
}