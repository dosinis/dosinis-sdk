using System;
using System.Threading.Tasks;
using DosinisSDK.Core;
using Unity.Notifications.iOS;

namespace DosinisSDK.Notifications
{
    public class IOSNotificationsManager : Module, INotificationsManager
    {
        private NotificationsConfig config;
        private NotificationsData data;

        public string OpenFromNotificationData { get; private set; }
        public bool Enabled => data.enabled;
        
        protected override async void OnInit(IApp app)
        {
            config = GetConfigAs<NotificationsConfig>();
            data = app.GetModule<IDataManager>().GetOrCreateData<NotificationsData>();
            
            using var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, false);
            
            while (req.IsFinished == false)
            {
                await Task.Yield();
            }   
            
            if (IsOpenedFromNotification(out string d))
            {
                OpenFromNotificationData = d;
            }
        }
        
        public void SetEnabled(bool value)
        {
            data.enabled = value;
        }

        public void ScheduleNotification(string title, string text, DateTime fireTime, TimeSpan? repeatInterval = null, string extraData = "")
        {
            if (Enabled == false)
                return;
            
            var notification = new iOSNotification
            {
                Title = title,
                Body = text,
                Trigger = BuildCalendarTrigger(fireTime, repeatInterval != null),
                Badge = 0,
                ShowInForeground = false,
                Data = extraData
            };

            iOSNotificationCenter.ScheduleNotification(notification);
            Log($"Notification {notification.Title} scheduled on: {fireTime}");
        }

        public void ScheduleNotification(string title, string text, long fireAfter, TimeSpan? repeatInterval = null, string extraData = "")
        {
            if (fireAfter < config.minDelayForNotification)
            {
                fireAfter = config.minDelayForNotification;
            }

            ScheduleNotification(title, text, DateTime.Now.AddSeconds(fireAfter));
        }
        
        private iOSNotificationCalendarTrigger BuildCalendarTrigger(DateTime dt, bool repeats)
        {
            return new iOSNotificationCalendarTrigger
            {
                Year = dt.Year,
                Month = dt.Month,
                Day = dt.Day,
                Hour = dt.Hour,
                Minute = dt.Minute,
                Second = dt.Second,
                Repeats = repeats
            };
        }

        public bool IsOpenedFromNotification(out string data)
        {
            var notification = iOSNotificationCenter.GetLastRespondedNotification();

            if (notification != null)
            {
                Log($"App opened from notification {notification.Title}, custom data {notification.Data} ");

                data = notification.Data;
                return true;
            }

            data = string.Empty;
            return false;
        }
    }
}
