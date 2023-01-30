using System;

namespace DosinisSDK.Core
{
    public interface IEventsManager : IModule
    {
        void Subscribe<T>(Action<T> action) where T : IEvent;
        void Unsubscribe<T>(Action<T> action) where T : IEvent;
        void Invoke<T>(T e) where T : IEvent;
    }
}
