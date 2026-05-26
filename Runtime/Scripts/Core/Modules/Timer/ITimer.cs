using System;

namespace DosinisSDK.Core
{
    public interface ITimer : IModule
    {
        ITimedAction Delay(float delay, Action done, bool realtime = false);
        ITimedAction CreateDelayAction(float delay, Action done, bool realtime = false);
        ITimedAction Sequence(Action done = null, params ITimedAction[] actions);
        ITimedAction Repeat(float frequency, int times, Action<int> onTick, float initDelay = 0f);
        ITimedAction SkipFrame(Action done);
        ITimedAction SkipFixedUpdate(Action done);
        ITimedAction WaitUntil(Func<bool> condition, Action done);
    }
}
