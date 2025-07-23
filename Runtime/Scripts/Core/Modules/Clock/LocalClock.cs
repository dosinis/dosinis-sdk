using System;

namespace DosinisSDK.Core
{
    public class LocalClock : Module, IClock
    {
        // UTC
        
        public long UtcNow => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        public long UtcNowMilliseconds => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public int DayOfYearUtc => DateTimeOffset.UtcNow.DayOfYear;
        public int LastActiveDayOfYearUtc => data.lastActiveDayOfYearUtc;
        public long LastTimeActiveUtc => data.lastTimeActiveUtc;
        
        // Local

        public long Now => DateTimeOffset.Now.ToUnixTimeSeconds();
        public long NowMilliseconds => DateTimeOffset.Now.ToUnixTimeMilliseconds();
        public int DayOfYear => DateTimeOffset.Now.DayOfYear;
        public int LastActiveDayOfYear => data.lastActiveDayOfYear;
        public long LastTimeActive => data.lastTimeActive;

        // Shared

        public long TimeInactive => UtcNow - LastTimeActiveUtc;
        
        // LocalClock

        private IApp app;
        private IDataManager dataManager;
        private LocalClockData data;
        
        protected override void OnInit(IApp app)
        {
            this.app = app;
            dataManager = app.GetModule<IDataManager>();
            
            data = dataManager.GetOrCreateData<LocalClockData>();

            if (data.lastTimeActive == 0)
            {
                data.lastTimeActive = UtcNow;
            }

            app.OnAppFocus += OnAppFocus;
            app.OnAppPaused += OnAppPaused;
            app.OnAppQuit += OnAppQuit;
        }

        protected override void OnDispose()
        {
            app.OnAppFocus -= OnAppFocus;
            app.OnAppPaused -= OnAppPaused;
            app.OnAppQuit -= OnAppQuit;
        }

        private void OnAppQuit()
        {
            data.lastTimeActive = Now;
            data.lastActiveDayOfYear = DayOfYear;
            data.lastTimeActiveUtc = UtcNow;
            data.lastActiveDayOfYearUtc = DayOfYearUtc;
            
            dataManager.SaveData(data);
        }

        private void OnAppPaused(bool paused)
        {
            if (paused)
            {
                data.lastTimeActive = UtcNow;
                data.lastActiveDayOfYear = DayOfYear;
                dataManager.SaveData(data);
            }
        }

        private void OnAppFocus(bool focus)
        {
            if (focus == false)
            {
                data.lastTimeActive = UtcNow;
                data.lastActiveDayOfYear = DayOfYear;
                dataManager.SaveData(data);
            }
        }
    }
}
