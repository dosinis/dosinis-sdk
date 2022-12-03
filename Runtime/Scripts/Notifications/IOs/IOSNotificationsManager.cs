using System;
using System.Threading.Tasks;
using DosinisSDK.Core;
using DosinisSDK.Utils;
using Unity.Notifications.iOS;

namespace DosinisSDK.Notifications
{
    public class IOSNotificationsManager : Module, INotificationsManager
    {
        private NotificationsConfig config;
        private NotificationsData data;

        public bool Enabled => data.enabled;
        
        protected override async void OnInit(IApp app)
        {
            config = GetConfigAs<NotificationsConfig>();
            data = app.GetModule<IDataManager>().RetrieveOrCreateData<NotificationsData>();
            
            using var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, false);
            
            while (req.IsFinished == false)
            {
                await Task.Yield();
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

            ScheduleNotification(title, text, Helper.GetDateTimeAfterSeconds(fireAfter));
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
    }
}
