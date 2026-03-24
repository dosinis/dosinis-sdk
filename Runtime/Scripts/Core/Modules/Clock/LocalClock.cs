using System;
using System.Collections.Generic;

namespace DosinisSDK.Core
{
    public class LocalClock : Module, IClock
    {
        // UTC

        public long UtcNow => GetUtcNowSeconds();
        public int UtcDayOfYear => GetUtcNowDate().DayOfYear;
        public long LastTimeActiveUtc => data.lastTimeActiveUtc;

        // Local

        public long Now => GetLocalNowSeconds();
        public int DayOfYear => GetLocalNowDate().DayOfYear;
        public long LastTimeActive => data.lastTimeActive;

        // Shared

        public long TimeInactive => UtcNow - LastTimeActiveUtc;
        public bool IsNewDay { get; private set; }
        public long TimeOffset { get; set; }

        // LocalClock

        private IApp app;
        private IDataManager dataManager;
        private LocalClockData data;
        private readonly List<string> newDayCache = new();

        protected override void OnInit(IApp app)
        {
            this.app = app;
            dataManager = app.GetModule<IDataManager>();

            data = dataManager.GetOrCreateData<LocalClockData>();

            if (data.lastTimeActive == 0)
            {
                data.lastTimeActive = UtcNow;
                data.lastTimeActiveUtc = UtcNow;
            }

            IsNewDay = data.previousDayOfYear != DayOfYear;
            data.previousDayOfYear = DayOfYear;

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
            data.lastTimeActiveUtc = UtcNow;

            dataManager.SaveData(data);
        }

        private void OnAppPaused(bool paused)
        {
            if (paused)
            {
                OnAppQuit();
            }
        }

        private void OnAppFocus(bool focus)
        {
            if (focus == false)
            {
                OnAppQuit();
            }
        }

        public bool IsNewDayCached(string forId)
        {
            if (IsNewDay == false)
                return false;

            if (string.IsNullOrEmpty(forId))
            {
                Warn("Id is null or empty!");
            }

            if (newDayCache.Contains(forId))
                return false;

            newDayCache.Add(forId);
            return true;
        }

        private long GetUtcNowSeconds()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds() + TimeOffset;
        }

        private long GetLocalNowSeconds()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds() + TimeOffset;
        }

        private DateTimeOffset GetUtcNowDate()
        {
            return DateTimeOffset.FromUnixTimeSeconds(GetUtcNowSeconds());
        }

        private DateTimeOffset GetLocalNowDate()
        {
            return DateTimeOffset.FromUnixTimeSeconds(GetLocalNowSeconds());
        }
    }
}