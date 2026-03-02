using System.Collections.Generic;

namespace Core.LevelingSystem
{
    public static class RewardFactory
    {
        public static RewardByLevelRuntime CreateRewardByLevel(RewardByLevel source)
        {
            return new RewardByLevelRuntime(source);
        }

        public static CyclicRewardRuntime CreateCyclicReward(CyclicReward source)
        {
            return new CyclicRewardRuntime(source);
        }

        public static List<RewardByLevelRuntime> CreateRewardsByLevelList(List<RewardByLevel> sources)
        {
            List<RewardByLevelRuntime> result = new(sources.Count);
            foreach (RewardByLevel source in sources)
                result.Add(CreateRewardByLevel(source));

            return result;
        }

        public static List<CyclicRewardRuntime> CreateCyclicRewardsList(List<CyclicReward> sources)
        {
            List<CyclicRewardRuntime> result = new(sources.Count);
            foreach (CyclicReward source in sources)
                result.Add(CreateCyclicReward(source));

            return result;
        }
    }
}
