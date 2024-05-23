using System;

namespace DosinisSDK.Core
{
    public class LocalClock : Module, IClock
    {
        public int DayOfYear => DateTimeOffset.UtcNow.DayOfYear;
        public long LastTimeActive => data.lastTimeActive;
        public long Now => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        public long NowMilliseconds => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public long TimeInactive => Now - LastTimeActive;
        
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
                data.lastTimeActive = Now;
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
            dataManager.SaveData(data);
        }

        private void OnAppPaused(bool paused)
        {
            if (paused)
            {
                data.lastTimeActive = Now;
                dataManager.SaveData(data);
            }
        }

        private void OnAppFocus(bool focus)
        {
            if (focus == false)
            {
                data.lastTimeActive = Now;
                dataManager.SaveData(data);
            }
        }
    }
}
