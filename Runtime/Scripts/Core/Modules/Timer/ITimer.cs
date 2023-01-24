using System;

namespace DosinisSDK.Core
{
    public interface ITimer : IModule
    {
        ITimedAction Delay(float delay, Action done);
        ITimedAction Repeat(float frequency, int times, Action<int> onTick, float initDelay = 0f);
        void SkipFrame(Action done);
        void SkipFixedUpdate(Action done);
        ITimedAction WaitUntil(Func<bool> condition, Action done);
    }
}
