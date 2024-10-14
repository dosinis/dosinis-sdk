namespace DosinisSDK.Core
{
    public interface IClock : IModule 
    {
        public int DayOfYear { get; }
        public int LastActiveDayOfYear { get; }
        public long LastTimeActive { get; }
        public long Now { get; }
        public long NowMilliseconds { get; }
        public long TimeInactive { get; }
        long GetTimeOfDay(int dayOfYear, int hour, int minute);
    }
}
