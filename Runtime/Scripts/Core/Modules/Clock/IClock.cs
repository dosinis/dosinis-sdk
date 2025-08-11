namespace DosinisSDK.Core
{
    public interface IClock : IModule 
    {
        // Local
        
        int DayOfYear { get; }
        int LastActiveDayOfYear { get; }
        long LastTimeActive { get; }
        long Now { get; }
        long NowMilliseconds { get; }
        
        // UTC
        
        int DayOfYearUtc { get; }
        int LastActiveDayOfYearUtc { get; }
        long LastTimeActiveUtc { get; }
        long UtcNow { get; }
        long UtcNowMilliseconds { get; }
        
        // Shared
        
        long TimeInactive { get; }
        bool IsNewDay { get; }
    }
}
