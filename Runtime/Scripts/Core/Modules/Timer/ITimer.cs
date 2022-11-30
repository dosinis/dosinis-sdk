using System;

namespace DosinisSDK.Core
{
    public interface ITimer : IModule
    {
        void Delay(float delay, Action done);
        void Repeat(float frequency, int times, Action<int> onTick, float initDelay = 0f, Func<bool> cancelFunc = null);
        void SkipFrame(Action done);
        void SkipFixedUpdate(Action done);
        void WaitUntil(Func<bool> condition, Action done);
    }
}
