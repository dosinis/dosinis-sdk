using System;

namespace DosinisSDK.Core
{
    public interface ITimer : IModule
    {
        void Delay(float delay, Action done);
        void Repeat(float frequency, int times, float initDelay, Action onTick);
        void SkipFrame(Action done);
        void WaitUntil(Func<bool> condition, Action done);
    }
}
