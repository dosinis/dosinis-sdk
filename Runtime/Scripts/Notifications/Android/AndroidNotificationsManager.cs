using System;
using System.Collections.Generic;
using DosinisSDK.Core;
using Unity.Notifications.Android;

namespace DosinisSDK.Notifications
{
    public class AndroidNotificationsManager : Module, INotificationsManager
    {
        private NotificationsConfig config;
        private NotificationsData data;
        private readonly Dictionary<string, int> cachedNotifications = new();

        public string OpenFromNotificationData { get; private set; }
        public bool Enabled => data.enabled;
        
        protected override void OnInit(IApp app)
        {
            config = GetConfigAs<NotificationsConfig>();
            data = app.GetModule<IDataManager>().GetOrCreateData<NotificationsData>();

            AndroidNotificationCenter.Initialize();
            
            //? This is for Android 13+ (when targeting API level 33)
            // if (Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS") == false)
            // {
            //     Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            // }
            
            AndroidNotificationCenter.RegisterNotificationChannel(BuildChannel(config.defaultChannel));
            
            AndroidNotificationCenter.OnNotificationReceived += OnNotificationReceived;
            
            if (IsOpenedFromNotification(out string d))
            {
                OpenFromNotificationData = d;
            }

            AndroidNotificationCenter.CancelAllNotifications();
        }

        public void SetEnabled(bool value)
        {
            data.enabled = value;
        }

        public void ScheduleNotification(string id, string title, string text, DateTime fireTime, TimeSpan? repeatInterval = null, string extraData = "")
        {
            if (Enabled == false)
                return;
            
            var notification = new AndroidNotification
            {
                Title = title,
                Text = text,
                FireTime = fireTime,
                SmallIcon = config.androidSmallIcon,
                LargeIcon = config.androidLargeIcon,
                IntentData = extraData,
                RepeatInterval = repeatInterval
            };
            
            if (cachedNotifications.ContainsKey(id))
            {
                AndroidNotificationCenter.CancelNotification(cachedNotifications[id]);
                cachedNotifications.Remove(id);
            }
            
            var notificationId = AndroidNotificationCenter.SendNotification(notification, config.defaultChannel.id);
            cachedNotifications.Add(id, notificationId);
            
            Log($"Notification {id} scheduled on: {notification.FireTime}");
        }

        public void ScheduleNotification(string id, string title, string text, long fireAfter, TimeSpan? repeatInterval = null, string extraData = "")
        {
            if (fireAfter < config.minDelayForNotification)
            {
                fireAfter = config.minDelayForNotification;
            }
            
            ScheduleNotification(id, title, text, DateTime.Now.AddSeconds(fireAfter), repeatInterval);
        }
        
        private void OnNotificationReceived(AndroidNotificationIntentData data)
        {
            AndroidNotificationCenter.CancelNotification(data.Id);
        }

        private AndroidNotificationChannel BuildChannel(NotificationChannelWrapper channelWrapper)
        {
            return new AndroidNotificationChannel
            {
                Id = channelWrapper.id,
                Name = channelWrapper.nameKey,
                Description = channelWrapper.description,
                Importance = (Importance)channelWrapper.importance
            };
        }

        public bool IsOpenedFromNotification(out string data)
        {
            var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();

            if (notificationIntentData != null)
            {
                var id = notificationIntentData.Id;
                var channel = notificationIntentData.Channel;
                var notification = notificationIntentData.Notification;
                Log($"App opened from notification: id {id}, channel: {channel}, intent data: {notification.IntentData}");

                data = notification.IntentData;
                return true;
            }

            data = string.Empty;
            return false;
        }
    }
}