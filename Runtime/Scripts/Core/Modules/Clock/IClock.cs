namespace DosinisSDK.Core
{
    public interface IClock : IModule 
    {
        // Local
        
        int DayOfYear { get; }
        long LastTimeActive { get; }
        long Now { get; }
        long NowMilliseconds { get; }
        
        // UTC
        
        long LastTimeActiveUtc { get; }
        long UtcNow { get; }
        long UtcNowMilliseconds { get; }
        
        // Shared
        
        long TimeInactive { get; }
        bool IsNewDay { get; }
        /// <summary>
        /// Return if it's a new day only once per session for specified id
        /// </summary>
        /// <param name="forId">Specified id used for caching</param>
        /// <returns></returns>
        bool IsNewDayCached(string forId);
    }
}
