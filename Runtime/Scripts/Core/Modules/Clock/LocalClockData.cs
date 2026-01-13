using System;

namespace DosinisSDK.Core
{
    [Serializable]
    public class LocalClockData : ModuleData, IGlobalData
    {
        public long lastTimeActive;
        public long lastTimeActiveUtc;
        public long previousDayOfYear;
    }
}
