namespace DosinisSDK.Core
{
    public interface IClock : IModule 
    {
        public int DayOfYear { get; }
        public long LastTimeActive { get; }
        public long Now { get; }
        public long NowMilliseconds { get; }
        public long TimeInactive { get; }
    }
}
