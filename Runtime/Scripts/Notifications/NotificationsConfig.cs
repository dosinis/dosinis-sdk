using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Notifications
{
    [CreateAssetMenu(fileName = "NotificationsConfig", menuName = "DosinisSDK/Configs/NotificationsConfig")]
    public class NotificationsConfig : ModuleConfig
    {
        public int minDelayForNotification;
        
        [Header("Android")]
        
        public NotificationChannelWrapper defaultChannel;

        public string androidSmallIcon = "small_icon";
        public string androidLargeIcon = "large_icon";
    }

    [System.Serializable]
    public class NotificationChannelWrapper
    {
        public string id;
        public string nameKey;
        public string description;
        public AndroidNotificationImportance importance = AndroidNotificationImportance.High;
    }

    public enum AndroidNotificationImportance
    {
        None = 0,
        Low = 2,
        Default = 3,
        High = 4,
    }
}