using System;

namespace DosinisSDK.Core
{
    public interface ITimer : IModule
    {
        ITimedAction Delay(float delay, Action done, bool realtime = true);
        ITimedAction CreateDelayAction(float delay, Action done, bool realtime = true);
        ITimedAction Sequence(Action done = null, params ITimedAction[] actions);
        ITimedAction Repeat(float frequency, int times, Action<int> onTick, float initDelay = 0f);
        void SkipFrame(Action done);
        void SkipFixedUpdate(Action done);
        ITimedAction WaitUntil(Func<bool> condition, Action done);
    }
}
