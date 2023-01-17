namespace DosinisSDK.Core
{
    public interface IClock : IModule 
    {
        public long LastTimeActive { get; }
        public long Now { get; }
        public long NowMilliseconds { get; }
        public long TimeInactive { get; }
    }
}
