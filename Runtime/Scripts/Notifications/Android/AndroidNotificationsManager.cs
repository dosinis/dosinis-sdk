using System;
using DosinisSDK.Core;
using DosinisSDK.Utils;
using Unity.Notifications.Android;

namespace DosinisSDK.Notifications
{
    public class AndroidNotificationsManager : Module, INotificationsManager
    {
        private NotificationsConfig config;
        private NotificationsData data;

        public bool Enabled => data.enabled;
        
        protected override void OnInit(IApp app)
        {
            config = GetConfigAs<NotificationsConfig>();
            data = app.GetModule<IDataManager>().RetrieveOrCreateData<NotificationsData>();

            AndroidNotificationCenter.Initialize();
            
            //? This is for Android 13+ (when targeting API level 33)
            // if (Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS") == false)
            // {
            //     Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            // }
            
            AndroidNotificationCenter.RegisterNotificationChannel(BuildChannel(config.defaultChannel));

            AndroidNotificationCenter.CancelAllNotifications();

            AndroidNotificationCenter.OnNotificationReceived += OnNotificationReceived;

            CheckIfAppOpenedFromNotification();
        }

        public void SetEnabled(bool value)
        {
            data.enabled = value;
        }

        public void ScheduleNotification(string title, string text, DateTime fireTime, TimeSpan? repeatInterval = null, string extraData = "")
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

            AndroidNotificationCenter.SendNotification(notification, config.defaultChannel.id);
            Log($"Notification {notification.Title} scheduled on: {notification.FireTime}");
        }

        public void ScheduleNotification(string title, string text, long fireAfter, TimeSpan? repeatInterval = null, string extraData = "")
        {
            if (fireAfter < config.minDelayForNotification)
            {
                fireAfter = config.minDelayForNotification;
            }
            
            ScheduleNotification(title, text, Helper.GetDateTimeAfterSeconds(fireAfter), repeatInterval);
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

        private void CheckIfAppOpenedFromNotification()
        {
            var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();

            if (notificationIntentData != null)
            {
                var id = notificationIntentData.Id;
                var channel = notificationIntentData.Channel;
                var notification = notificationIntentData.Notification;
                Log($"App opened from notification: id {id}, channel: {channel}, intent data:");
            }
        }
    }
}