using System;

namespace DosinisSDK.Core
{
    [Serializable]
    public class LocalClockData : ModuleData
    {
        public long lastTimeActive;
        public long lastTimeActiveUtc;
        public int lastActiveDayOfYear;
        public int lastActiveDayOfYearUtc;
    }
}
