using System;

namespace DosinisSDK.Core
{
    public class LocalClock : Module, IClock
    {
        public long LastTimeActive => data.lastTimeActive;
        public long Now => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        public long NowMilliseconds => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public long TimeInactive => Now - LastTimeActive;
        
        // LocalClock
        
        private LocalClockData data;
        
        protected override void OnInit(IApp app)
        {
            var dataManager = app.GetModule<IDataManager>();
            
            data = dataManager.RetrieveOrCreateData<LocalClockData>();

            if (data.lastTimeActive == 0)
            {
                data.lastTimeActive = Now;
            }

            app.OnAppFocus += focus =>
            {
                if (focus == false)
                {
                    data.lastTimeActive = Now;
                    dataManager.SaveData(data);
                }
            };

            app.OnAppPaused += paused =>
            {
                if (paused)
                {
                    data.lastTimeActive = Now;
                    dataManager.SaveData(data);
                }
            };

            app.OnAppQuit += () =>
            {
                data.lastTimeActive = Now;
                dataManager.SaveData(data);
            };
        }
    }
}
