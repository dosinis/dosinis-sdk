using System;
using DosinisSDK.Core;

namespace DosinisSDK.Notifications
{
    public interface INotificationsManager : IModule
    {
        bool Enabled { get; }
        string OpenFromNotificationData { get; }
        void SetEnabled(bool value);
        void ScheduleNotification(string id, string title, string text, DateTime fireTime, TimeSpan? repeatInterval = null, string extraData = "");
        void ScheduleNotification(string id, string title, string text, long fireAfter, TimeSpan? repeatInterval = null, string extraData = "");
        bool IsOpenedFromNotification(out string data);
    }
}
