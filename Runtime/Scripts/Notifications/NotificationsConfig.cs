using DosinisSDK.Core;
using Unity.Notifications.Android;
using UnityEngine;

namespace DosinisSDK.Notifications
{
    [CreateAssetMenu(fileName = "NotificationsConfig", menuName = "DosinisSDK/Configs/NotificationsConfig")]
    public class NotificationsConfig : ModuleConfig
    {
        public NotificationChannelWrapper defaultChannel;
        public int minDelayForNotification;

        public string androidSmallIcon = "small_icon";
        public string androidLargeIcon = "large_icon";
    }

    [System.Serializable]
    public class NotificationChannelWrapper
    {
        public string id;
        public string nameKey;
        public string description;
        public Importance importance = Importance.High;
    }
}