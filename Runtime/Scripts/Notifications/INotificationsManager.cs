using System;
using DosinisSDK.Core;

namespace DosinisSDK.Notifications
{
    public interface INotificationsManager : IModule
    {
        void ScheduleNotification(string title, string text, DateTime fireTime, TimeSpan? repeatInterval = null, string extraData = "");
        void ScheduleNotification(string title, string text, long fireAfter, TimeSpan? repeatInterval = null, string extraData = "");
    }
}