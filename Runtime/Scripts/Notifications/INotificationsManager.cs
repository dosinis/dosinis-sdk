using System;
using DosinisSDK.Core;

namespace DosinisSDK.Notifications
{
    public interface INotificationsManager : IModule
    {
        bool Enabled { get; }
        void SetEnabled(bool value);
        void ScheduleNotification(string title, string text, DateTime fireTime, TimeSpan? repeatInterval = null, string extraData = "");
        void ScheduleNotification(string title, string text, long fireAfter, TimeSpan? repeatInterval = null, string extraData = "");
    }
}
