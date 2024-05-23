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
        private readonly List<ReturnNotification> returnNotifications = new();
        
        public string OpenFromNotificationData { get; private set; }
        public bool Enabled => data.enabled;

        private IApp app;

        protected override async void OnInit(IApp app)
        {
            this.app = app;
            config = GetConfigAs<NotificationsConfig>();
            data = app.DataManager.GetOrCreateData<NotificationsData>();
            
            using var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, false);
            
            while (req.IsFinished == false)
            {
                await Task.Yield();
            }   
            
            if (IsOpenedFromNotification(out string d))
            {
                OpenFromNotificationData = d;
            }
            
            foreach (var notification in config.returnNotifications)
            {
                if (notification.scheduleType == ScheduleType.OnAppStart)
                {
                    ScheduleNotification(notification.id, notification.title, notification.text, notification.fireAfter, null, notification.extraData);
                }
                else
                {
                    returnNotifications.Add(notification);
                }
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
        
        private void ScheduleReturnNotifications()
        {
            foreach (var notification in returnNotifications)
            {
                ScheduleNotification(notification.id, notification.title, notification.text, notification.fireAfter, null, notification.extraData);
            }
        }
        
        private void OnAppPaused(bool paused)
        {
            if (paused)
            {
                ScheduleReturnNotifications();
            }
        }

        private void OnAppFocus(bool focus)
        {
            if (!focus)
            {
                ScheduleReturnNotifications();
            }
        }

        private void OnAppQuit()
        {
            ScheduleReturnNotifications();
        }
        
        public void SetEnabled(bool value)
        {
            data.enabled = value;
        }

        public void ScheduleNotification(string id, string title, string text, DateTime fireTime, TimeSpan? repeatInterval = null, string extraData = "")
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
            
            // TODO: cancel duplicate notifications

            iOSNotificationCenter.ScheduleNotification(notification);
            Log($"Notification {notification.Title} scheduled on: {fireTime}");
        }

        public void ScheduleNotification(string id, string title, string text, long fireAfter, TimeSpan? repeatInterval = null, string extraData = "")
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
